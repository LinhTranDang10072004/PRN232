using ClientMVC.Models.Corporate;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Corporate.Controllers
{
    [Area("Corporate")]
    [Authorize(Roles = "Admin")]
    public class AdminStaffsController : Controller
    {
        private readonly ICorporateApiClient _api;

        public AdminStaffsController(ICorporateApiClient api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Index(bool? showCreate = null)
        {
            var vm = new StaffManageViewModel { ShowCreateModal = showCreate == true };
            var (list, err) = await _api.GetStaffsAsync();
            if (err != null) vm.ErrorMessage = err;
            else vm.Staffs = list ?? new();

            if (TempData["Success"] is string ok) vm.SuccessMessage = ok;
            if (TempData["Error"] is string fail) vm.ErrorMessage = fail;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "Form")] CreateStaffFormModel form)
        {
            if (!ModelState.IsValid)
            {
                var vm = new StaffManageViewModel { Form = form, ShowCreateModal = true };
                var (list, _) = await _api.GetStaffsAsync();
                vm.Staffs = list ?? new();
                return View("Index", vm);
            }

            var (_, error) = await _api.CreateStaffAsync(form);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã tạo tài khoản Staff.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var (ok, message, error) = await _api.SetStaffActiveAsync(id, false);
            TempData[ok ? "Success" : "Error"] = message ?? error ?? "Không thể vô hiệu hóa.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var (ok, message, error) = await _api.SetStaffActiveAsync(id, true);
            TempData[ok ? "Success" : "Error"] = message ?? error ?? "Không thể kích hoạt.";
            return RedirectToAction(nameof(Index));
        }
    }
}
