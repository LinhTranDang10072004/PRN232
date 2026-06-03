using System.Security.Claims;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    /// <summary>
    /// Nhánh 1 (Role User): CRUD chi tiêu cá nhân — chỉ thấy/sửa dữ liệu của chính mình.
    /// </summary>
    [ApiController]
    [Route("api/personal/expenses")]
    [Authorize(Roles = "User")]
    public class PersonalExpensesController : ControllerBase
    {
        private readonly IPersonalExpenseService _expenseService;

        public PersonalExpensesController(IPersonalExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        /// <summary>
        /// Danh sách chi tiêu của tôi — hỗ trợ OData: $filter, $orderby, $top, $skip, $select, $count.
        /// Ví dụ: ?$filter=Amount gt 100000&amp;$orderby=ExpenseDate desc&amp;$top=10
        /// </summary>
        [HttpGet]
        [EnableQuery(PageSize = 50)]
        public IActionResult GetAll()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            return Ok(_expenseService.GetMyExpenses(userId.Value));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ExpenseResponse>> GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var expense = await _expenseService.GetByIdAsync(userId.Value, id);
            if (expense == null)
                return NotFound(new { message = "Không tìm thấy khoản chi." });

            return Ok(expense);
        }

        [HttpPost]
        public async Task<ActionResult<ExpenseResponse>> Create([FromBody] CreateExpenseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var (result, error) = await _expenseService.CreateAsync(userId.Value, request);
            if (error != null)
                return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<ExpenseResponse>> Update(int id, [FromBody] UpdateExpenseRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var (result, error) = await _expenseService.UpdateAsync(userId.Value, id, request);
            if (error != null)
            {
                if (error.Contains("Không tìm thấy"))
                    return NotFound(new { message = error });
                return BadRequest(new { message = error });
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var (success, error) = await _expenseService.DeleteAsync(userId.Value, id);
            if (!success)
                return NotFound(new { message = error });

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
