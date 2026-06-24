using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/personal/reports")]
    public class PersonalReportsController : ControllerBase
    {
        private readonly IPersonalReportService _service;

        public PersonalReportsController(IPersonalReportService service) => _service = service;

        [HttpGet("monthly")]
        public async Task<IActionResult> Monthly([FromQuery] int month, [FromQuery] int year) =>
            Ok(await _service.GetMonthlySummaryAsync(User.GetUserId(), month, year));

        [HttpGet("by-category")]
        public async Task<IActionResult> ByCategory([FromQuery] int month, [FromQuery] int year) =>
            Ok(await _service.GetByCategoryAsync(User.GetUserId(), month, year));

        [HttpGet("by-wallet")]
        public async Task<IActionResult> ByWallet([FromQuery] int month, [FromQuery] int year) =>
            Ok(await _service.GetByWalletAsync(User.GetUserId(), month, year));

        [HttpGet("budget-status")]
        public async Task<IActionResult> BudgetStatus([FromQuery] int month, [FromQuery] int year) =>
            Ok(await _service.GetBudgetStatusAsync(User.GetUserId(), month, year));

        [HttpGet("yearly")]
        public async Task<IActionResult> Yearly([FromQuery] int year) =>
            Ok(await _service.GetYearlyReportAsync(User.GetUserId(), year));
    }
}
