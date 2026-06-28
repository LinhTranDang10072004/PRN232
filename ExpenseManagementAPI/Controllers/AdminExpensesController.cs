using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/expenses")]
    public class AdminExpensesController : ControllerBase
    {
        private readonly IAdminExpenseService _service;

        public AdminExpensesController(IAdminExpenseService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetForCompany(User.GetCompanyId()));

        [HttpGet("dashboard")]
        public async Task<IActionResult> Dashboard() =>
            Ok(await _service.GetDashboardAsync(User.GetCompanyId()));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var (success, data, error) = await _service.GetByIdAsync(User.GetCompanyId(), id);
            return success ? Ok(data) : NotFound(new { message = error });
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var (success, error) = await _service.ApproveAsync(
                User.GetUserId(), User.GetCompanyId(), id);
            return success ? Ok(new { message = "Đã duyệt phiếu chi." }) : BadRequest(new { message = error });
        }

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectExpenseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, error) = await _service.RejectAsync(
                User.GetUserId(), User.GetCompanyId(), id, request.Comment);
            return success ? Ok(new { message = "Đã từ chối phiếu chi." }) : BadRequest(new { message = error });
        }
    }
}
