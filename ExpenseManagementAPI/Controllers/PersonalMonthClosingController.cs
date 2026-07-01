using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/personal/month-closing")]
    public class PersonalMonthClosingController : ControllerBase
    {
        private readonly IPersonalMonthClosingService _service;

        public PersonalMonthClosingController(IPersonalMonthClosingService service) => _service = service;

        [HttpGet("preview")]
        public async Task<IActionResult> Preview([FromQuery] int month, [FromQuery] int year) =>
            Ok(await _service.GetClosingPreviewAsync(User.GetUserId(), month, year));

        [HttpGet("status")]
        public async Task<IActionResult> Status([FromQuery] int month, [FromQuery] int year) =>
            Ok(await _service.GetClosingStatusAsync(User.GetUserId(), month, year));

        [HttpGet("warn-previous")]
        public async Task<IActionResult> WarnPrevious([FromQuery] int month, [FromQuery] int year) =>
            Ok(new { shouldWarn = await _service.ShouldWarnPreviousMonthNotClosedAsync(User.GetUserId(), month, year) });

        [HttpPost("close")]
        public async Task<IActionResult> Close([FromBody] CloseMonthRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.CloseMonthAsync(
                User.GetUserId(), request.Month, request.Year, request.Notes);
            return success ? Ok(data) : BadRequest(new { message = error });
        }
    }
}
