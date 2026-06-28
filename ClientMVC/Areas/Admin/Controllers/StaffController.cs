using ClientMVC.Models.Admin;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Admin.Controllers
{
    public class StaffController : AdminBaseController
    {
        private readonly IAdminApiClient _api;

        public StaffController(IAdminApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var staff = await _api.GetStaffAsync();
            return View(staff);
        }

        public IActionResult Create() => View(new CreateStaffFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateStaffFormModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (ok, error) = await _api.CreateStaffAsync(model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Tạo nhân viên thất bại.");
                return View(model);
            }

            SetSuccess("Đã tạo tài khoản Staff.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActive(int id, bool isActive)
        {
            var (ok, error) = await _api.UpdateStaffStatusAsync(id, isActive);
            if (!ok) SetError(error ?? "Cập nhật thất bại.");
            else SetSuccess(isActive ? "Đã kích hoạt nhân viên." : "Đã khóa nhân viên.");
            return RedirectToAction(nameof(Index));
        }
    }
}
