using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "Staff")]
    [ApiController]
    [Route("api/staff/notifications")]
    public class StaffNotificationsController : ControllerBase
    {
        private readonly IStaffNotificationQueryService _service;

        public StaffNotificationsController(IStaffNotificationQueryService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 50)]
        public IActionResult Get() => Ok(_service.GetForUser(User.GetUserId()));

        [HttpGet("unread-count")]
        public async Task<IActionResult> UnreadCount() =>
            Ok(new { count = await _service.GetUnreadCountAsync(User.GetUserId()) });

        [HttpPut("{id:int}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var (success, error) = await _service.MarkAsReadAsync(User.GetUserId(), id);
            return success ? NoContent() : NotFound(new { message = error });
        }

        [HttpPut("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _service.MarkAllAsReadAsync(User.GetUserId());
            return NoContent();
        }
    }
}
