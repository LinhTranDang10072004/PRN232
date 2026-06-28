using ClientMVC.Models.Admin;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Admin.Controllers
{
    public class AccountsController : AdminBaseController
    {
        private readonly IAdminApiClient _api;

        public AccountsController(IAdminApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var items = await _api.GetAccountsAsync();
            return View(items.OrderBy(a => a.Name).ToList());
        }

        public IActionResult Create() => View(new AdminAccountFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminAccountFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, error) = await _api.CreateAccountAsync(model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Thêm thất bại.");
                return View(model);
            }
            SetSuccess("Đã thêm tài khoản.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var items = await _api.GetAccountsAsync();
            var item = items.FirstOrDefault(a => a.Id == id);
            if (item == null) return NotFound();
            return View(new AdminAccountFormModel
            {
                Id = item.Id,
                Name = item.Name,
                AccountNumber = item.AccountNumber,
                IsActive = item.IsActive
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminAccountFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, error) = await _api.UpdateAccountAsync(id, model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Cập nhật thất bại.");
                return View(model);
            }
            SetSuccess("Đã cập nhật tài khoản.");
            return RedirectToAction(nameof(Index));
        }
    }
}
