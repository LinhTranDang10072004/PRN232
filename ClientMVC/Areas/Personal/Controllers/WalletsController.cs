using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class WalletsController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public WalletsController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index() => View(await _api.GetWalletsAsync());

        public IActionResult Create() => View(new WalletFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(WalletFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, _, error) = await _api.CreateWalletAsync(model);
            if (!ok) { model.ErrorMessage = error; return View(model); }
            SetSuccess("Đã tạo ví.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var wallet = await _api.GetWalletAsync(id);
            if (wallet == null) return NotFound();
            return View(new WalletFormModel
            {
                Id = wallet.Id,
                Name = wallet.Name,
                Type = wallet.Type ?? "Cash",
                InitialBalance = wallet.Balance
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, WalletFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, _, error) = await _api.UpdateWalletAsync(id, model);
            if (!ok) { model.ErrorMessage = error; return View(model); }
            SetSuccess("Đã cập nhật ví.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var (ok, error) = await _api.DeactivateWalletAsync(id);
            if (!ok) SetError(error ?? "Thất bại.");
            else SetSuccess("Đã ngưng sử dụng ví.");
            return RedirectToAction(nameof(Index));
        }
    }
}
