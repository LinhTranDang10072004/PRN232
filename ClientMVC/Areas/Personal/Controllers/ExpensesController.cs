using BusinessObjects.Enums;
using ClientMVC.Helpers;
using ClientMVC.Models;
using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class ExpensesController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public ExpensesController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Calendar(int? year, int? month, int? day)
        {
            var y = year ?? DateTime.Today.Year;
            var m = month ?? DateTime.Today.Month;
            var expenses = await _api.GetExpensesForMonthAsync(y, m);
            var model = CalendarHelper.Build(y, m, expenses, day);
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(model);
        }

        public async Task<IActionResult> Index(ExpenseFilterModel filter)
        {
            await LoadFilterLookupsAsync();
            var oDataFilter = ODataExpenseFilterBuilder.Build(filter, ExpenseFilterScope.Personal);
            var expenses = await _api.GetExpensesAsync(oDataFilter);
            SetFilterViewBag(filter, ExpenseFilterScope.Personal, expenses);
            return View(expenses);
        }

        public async Task<IActionResult> Create(DateTime? date)
        {
            await LoadLookupsAsync();
            return View(new ExpenseFormModel
            {
                ExpenseDate = date?.Date ?? DateTime.Today
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseFormModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(model);
            }

            var (ok, data, error) = await _api.CreateExpenseAsync(model);
            if (!ok)
            {
                model.ErrorMessage = error;
                await LoadLookupsAsync();
                return View(model);
            }

            if (data?.BudgetExceeded == true)
            {
                SetWarning($"🔴 Đã thêm khoản chi nhưng VƯỢT NGÂN SÁCH! Vượt {data.BudgetOverflowAmount:N0} so với hạn mức tháng này.");
            }
            else
            {
                SetSuccess("Đã thêm khoản chi.");
            }
            return RedirectToAction(nameof(Calendar), new { year = model.ExpenseDate.Year, month = model.ExpenseDate.Month, day = model.ExpenseDate.Day });
        }

        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _api.GetExpenseAsync(id);
            if (expense == null)
                return NotFound();

            await LoadLookupsAsync();
            return View(new ExpenseFormModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate?.Date ?? DateTime.Today,
                CategoryId = expense.CategoryId ?? 0,
                WalletId = expense.WalletId ?? 0
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ExpenseFormModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(model);
            }

            var (ok, data, error) = await _api.UpdateExpenseAsync(id, model);
            if (!ok)
            {
                model.ErrorMessage = error;
                await LoadLookupsAsync();
                return View(model);
            }

            if (data?.BudgetExceeded == true)
            {
                SetWarning($"🔴 Đã cập nhật nhưng VƯỢT NGÂN SÁCH! Vượt {data.BudgetOverflowAmount:N0} so với hạn mức tháng này.");
            }
            else
            {
                SetSuccess("Đã cập nhật khoản chi.");
            }
            return RedirectToAction(nameof(Calendar), new { year = model.ExpenseDate.Year, month = model.ExpenseDate.Month, day = model.ExpenseDate.Day });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int? year, int? month, int? day)
        {
            var (ok, error) = await _api.DeleteExpenseAsync(id);
            if (!ok)
                SetError(error ?? "Xóa thất bại.");
            else
                SetSuccess("Đã xóa khoản chi.");

            return RedirectToAction(nameof(Calendar), new { year, month, day });
        }

        private async Task LoadLookupsAsync()
        {
            var categories = await _api.GetCategoriesAsync();
            var wallets = await _api.GetWalletsAsync();

            ViewBag.Categories = new SelectList(
                categories.Where(c => c.Status == CategoryStatus.Active),
                "Id", "Name");

            ViewBag.Wallets = new SelectList(
                wallets.Where(w => w.Status != "Inactive"),
                "Id", "Name");
        }

        private async Task LoadFilterLookupsAsync()
        {
            var categories = await _api.GetCategoriesAsync();
            var wallets = await _api.GetWalletsAsync();

            ViewBag.Categories = new SelectList(
                categories.Where(c => c.Status == CategoryStatus.Active),
                "Id", "Name");

            ViewBag.Wallets = new SelectList(wallets, "Id", "Name");
        }

        private void SetFilterViewBag(ExpenseFilterModel filter, ExpenseFilterScope scope, List<ExpenseDto> expenses)
        {
            ViewBag.Filter = filter;
            ViewBag.FilterConfig = new ExpenseFilterViewConfig
            {
                Area = "Personal",
                ShowWallet = scope == ExpenseFilterScope.Personal
            };
            ViewBag.ResultCount = expenses.Count;
            ViewBag.TotalAmount = expenses.Sum(e => e.Amount);
        }
    }
}
