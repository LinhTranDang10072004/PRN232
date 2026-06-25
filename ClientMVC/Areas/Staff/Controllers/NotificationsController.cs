using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Staff.Controllers
{
    public class NotificationsController : StaffBaseController
    {
        private readonly IStaffApiClient _api;

        public NotificationsController(IStaffApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(await _api.GetNotificationsAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkRead(int id)
        {
            await _api.MarkNotificationReadAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllRead()
        {
            await _api.MarkAllNotificationsReadAsync();
            SetSuccess("Đã đánh dấu tất cả đã đọc.");
            return RedirectToAction(nameof(Index));
        }
    }
}
