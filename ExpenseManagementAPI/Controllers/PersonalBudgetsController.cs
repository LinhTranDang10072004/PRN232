using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/personal/budgets")]
    public class PersonalBudgetsController : ControllerBase
    {
        private readonly IPersonalBudgetService _service;

        public PersonalBudgetsController(IPersonalBudgetService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get([FromQuery] int? month, [FromQuery] int? year) =>
            Ok(_service.GetForUser(User.GetUserId(), month, year));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var (success, data, error) = await _service.GetByIdAsync(User.GetUserId(), id);
            return success ? Ok(data) : NotFound(new { message = error });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBudgetRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.CreateAsync(User.GetUserId(), request);
            return success ? CreatedAtAction(nameof(GetById), new { id = data!.Id }, data) : BadRequest(new { message = error });
        }

        [HttpPut("{id:int}/limit")]
        public async Task<IActionResult> UpdateLimit(int id, [FromBody] UpdateBudgetLimitRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.UpdateLimitAsync(User.GetUserId(), id, request);
            return success ? Ok(data) : BadRequest(new { message = error });
        }
    }
}
