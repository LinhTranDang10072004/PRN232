using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class ProfileController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public ProfileController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var profile = await _api.GetProfileAsync();
            if (profile == null) return RedirectToAction("Login", "Auth", new { area = "" });

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
            return View(model);
        }

        public IActionResult ChangePassword() => View(new ChangePasswordFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, error) = await _api.ChangePasswordAsync(model);
            if (!ok) model.ErrorMessage = error;
            else model.SuccessMessage = "Đã đổi mật khẩu.";
            return View(model);
        }
    }
}
