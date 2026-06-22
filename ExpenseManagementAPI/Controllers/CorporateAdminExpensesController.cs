using System.Security.Claims;
using ExpenseManagementAPI.DTOs.Corporate;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/corporate/admin/expenses")]
    [Authorize(Roles = "Admin")]
    public class CorporateAdminExpensesController : ControllerBase
    {
        private readonly ICorporateExpenseService _service;

        public CorporateAdminExpensesController(ICorporateExpenseService service)
        {
            _service = service;
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryOptionDto>>> GetCategories() =>
            Ok(await _service.GetCorporateCategoriesAsync());

        [HttpGet]
        [EnableQuery(PageSize = 200)]
        public IActionResult GetAll()
        {
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();
            return Ok(_service.GetForAdmin(adminId.Value));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CorporateExpenseResponse>> GetById(int id)
        {
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();
            var item = await _service.GetByIdForAdminAsync(adminId.Value, id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id)
        {
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();
            var (ok, error) = await _service.ApproveAsync(adminId.Value, id);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Đã duyệt khoản chi." });
        }

        [HttpPost("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] RejectExpenseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();
            var (ok, error) = await _service.RejectAsync(adminId.Value, id, request.Reason);
            if (!ok) return BadRequest(new { message = error });
            return Ok(new { message = "Đã từ chối khoản chi." });
        }

        private int? GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
