using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class BudgetsController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public BudgetsController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index(int? month, int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;
            ViewBag.Month = m;
            ViewBag.Year = y;
            return View(await _api.GetBudgetsAsync(m, y));
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync();
            await LoadWalletsAsync();
            return View(new BudgetFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BudgetFormModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                await LoadWalletsAsync();
                return View(model);
            }

            var (ok, _, error) = await _api.CreateBudgetAsync(model);
            if (!ok)
            {
                model.ErrorMessage = error;
                await LoadCategoriesAsync();
                await LoadWalletsAsync();
                return View(model);
            }

            SetSuccess("Đã tạo budget.");
            return RedirectToAction(nameof(Index), new { month = model.Month, year = model.Year });
        }

        public async Task<IActionResult> EditLimit(int id)
        {
            var budgets = await _api.GetBudgetsAsync();
            var budget = budgets.FirstOrDefault(b => b.Id == id);
            if (budget == null) return NotFound();
            ViewBag.BudgetName = budget.Name;
            return View(new BudgetFormModel
            {
                LimitAmount = budget.LimitAmount,
                Month = budget.Month ?? DateTime.Today.Month,
                Year = budget.Year ?? DateTime.Today.Year
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLimit(int id, BudgetFormModel model)
        {
            var (ok, _, error) = await _api.UpdateBudgetLimitAsync(id, model.LimitAmount);
            if (!ok)
            {
                model.ErrorMessage = error;
                return View(model);
            }

            SetSuccess("Đã cập nhật hạn mức.");
            return RedirectToAction(nameof(Index), new { month = model.Month, year = model.Year });
        }

        private async Task LoadCategoriesAsync()
        {
            var categories = await _api.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(
                categories.Where(c => c.Status == BusinessObjects.Enums.CategoryStatus.Active),
                "Id", "Name");
        }

        private async Task LoadWalletsAsync()
        {
            var wallets = await _api.GetWalletsAsync();
            ViewBag.Wallets = new SelectList(
                wallets.Where(w => w.Status != "Inactive"),
                "Id", "Name");
        }
    }
}
