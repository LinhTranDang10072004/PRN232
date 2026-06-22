using System.Security.Claims;
using ExpenseManagementAPI.DTOs.Corporate;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/corporate/staff/expenses")]
    [Authorize(Roles = "Staff")]
    public class CorporateStaffExpensesController : ControllerBase
    {
        private readonly ICorporateExpenseService _service;

        public CorporateStaffExpensesController(ICorporateExpenseService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult GetAll()
        {
            var staffId = GetUserId();
            if (staffId == null) return Unauthorized();
            return Ok(_service.GetForStaff(staffId.Value));
        }

        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryOptionDto>>> GetCategories() =>
            Ok(await _service.GetCorporateCategoriesAsync());

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CorporateExpenseResponse>> GetById(int id)
        {
            var staffId = GetUserId();
            if (staffId == null) return Unauthorized();
            var item = await _service.GetByIdForStaffAsync(staffId.Value, id);
            if (item == null) return NotFound(new { message = "Không tìm thấy." });
            return Ok(item);
        }

        [HttpPost]
        public async Task<ActionResult<CorporateExpenseResponse>> Create([FromBody] CorporateExpenseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var staffId = GetUserId();
            if (staffId == null) return Unauthorized();
            var (result, error) = await _service.CreateForStaffAsync(staffId.Value, request);
            if (error != null) return BadRequest(new { message = error });
            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CorporateExpenseResponse>> Update(int id, [FromBody] CorporateExpenseRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var staffId = GetUserId();
            if (staffId == null) return Unauthorized();
            var (result, error) = await _service.UpdateForStaffAsync(staffId.Value, id, request);
            if (error != null)
                return error.Contains("Không tìm thấy") ? NotFound(new { message = error }) : BadRequest(new { message = error });
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var staffId = GetUserId();
            if (staffId == null) return Unauthorized();
            var (ok, error) = await _service.DeleteForStaffAsync(staffId.Value, id);
            if (!ok) return BadRequest(new { message = error });
            return NoContent();
        }

        private int? GetUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
