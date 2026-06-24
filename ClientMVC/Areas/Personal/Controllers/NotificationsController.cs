using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class NotificationsController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public NotificationsController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index() => View(await _api.GetNotificationsAsync());

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
