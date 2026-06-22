using System.Security.Claims;
using ExpenseManagementAPI.DTOs.Corporate;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/corporate/admin/staffs")]
    [Authorize(Roles = "Admin")]
    public class CorporateAdminStaffsController : ControllerBase
    {
        private readonly ICorporateStaffService _service;

        public CorporateAdminStaffsController(ICorporateStaffService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffResponse>>> GetAll()
        {
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();
            return Ok(await _service.GetStaffListAsync(adminId.Value));
        }

        [HttpPost]
        public async Task<ActionResult<StaffResponse>> Create([FromBody] CreateStaffRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();
            var (result, error) = await _service.CreateStaffAsync(adminId.Value, request);
            if (error != null) return BadRequest(new { message = error });
            return Ok(result);
        }

        /// <summary>Kích hoạt / vô hiệu hóa Staff (sa thải = IsActive false).</summary>
        [HttpPut("{id:int}/active")]
        public async Task<ActionResult<StaffResponse>> SetActive(int id, [FromBody] SetStaffActiveRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var adminId = GetUserId();
            if (adminId == null) return Unauthorized();

            var (result, error) = await _service.SetStaffActiveAsync(adminId.Value, id, request.IsActive);
            if (result == null)
                return NotFound(new { message = error });
            return Ok(new { message = error, staff = result });
        }

        private int? GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
