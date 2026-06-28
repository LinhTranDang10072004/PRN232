using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/staff")]
    public class AdminStaffController : ControllerBase
    {
        private readonly IAdminStaffService _service;

        public AdminStaffController(IAdminStaffService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _service.GetStaffAsync(User.GetCompanyId()));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateStaffRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.CreateStaffAsync(User.GetCompanyId(), request);
            return success
                ? CreatedAtAction(nameof(Get), new { id = data!.Id }, data)
                : BadRequest(new { message = error });
        }

        [HttpPut("{id:int}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStaffStatusRequest request)
        {
            var (success, error) = await _service.UpdateStaffStatusAsync(
                User.GetCompanyId(), id, request.IsActive);
            return success ? Ok(new { message = "Đã cập nhật trạng thái." }) : BadRequest(new { message = error });
        }
    }
}
