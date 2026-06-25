using ClientMVC.Models.Staff;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Staff.Controllers
{
    public class ProfileController : StaffBaseController
    {
        private readonly IStaffApiClient _api;

        public ProfileController(IStaffApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var profile = await _api.GetProfileAsync();
            if (profile == null)
                return RedirectToAction("Login", "Auth", new { area = "" });

            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(new ProfileFormModel
            {
                Email = profile.Email,
                FullName = profile.FullName
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileFormModel model)
        {
            var (ok, error) = await _api.UpdateProfileAsync(model);
            if (!ok) model.ErrorMessage = error;
            else model.SuccessMessage = "Đã cập nhật hồ sơ.";

            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(new ChangePasswordFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordFormModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (ok, error) = await _api.ChangePasswordAsync(model);
            if (!ok) model.ErrorMessage = error;
            else model.SuccessMessage = "Đã đổi mật khẩu.";

            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(model);
        }
    }
}
