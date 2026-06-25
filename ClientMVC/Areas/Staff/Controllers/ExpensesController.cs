using BusinessObjects.Enums;
using ClientMVC.Models.Staff;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientMVC.Areas.Staff.Controllers
{
    public class ExpensesController : StaffBaseController
    {
        private readonly IStaffApiClient _api;

        public ExpensesController(IStaffApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var expenses = await _api.GetExpensesAsync();
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(expenses.OrderByDescending(e => e.CreatedAt).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var expense = await _api.GetExpenseAsync(id);
            if (expense == null)
                return NotFound();

            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(expense);
        }

        public async Task<IActionResult> Create()
        {
            await LoadLookupsAsync();
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(new StaffExpenseFormModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StaffExpenseFormModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(model);
            }

            var (ok, _, error) = await _api.CreateExpenseAsync(model);
            if (!ok)
            {
                model.ErrorMessage = error;
                await LoadLookupsAsync();
                return View(model);
            }

            SetSuccess("Đã gửi phiếu chi chờ duyệt.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var expense = await _api.GetExpenseAsync(id);
            if (expense == null)
                return NotFound();

            if (expense.Status == ExpenseStatus.Approved)
            {
                SetError("Phiếu đã được duyệt, không thể sửa.");
                return RedirectToAction(nameof(Details), new { id });
            }

            await LoadLookupsAsync();
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(new StaffExpenseFormModel
            {
                Id = expense.Id,
                Title = expense.Title,
                Description = expense.Description,
                Amount = expense.Amount,
                ExpenseDate = expense.ExpenseDate?.Date ?? DateTime.Today,
                CategoryId = expense.CategoryId ?? 0,
                AccountId = expense.AccountId ?? 0,
                IsLocked = expense.Status == ExpenseStatus.Approved
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StaffExpenseFormModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadLookupsAsync();
                return View(model);
            }

            var (ok, _, error) = await _api.UpdateExpenseAsync(id, model);
            if (!ok)
            {
                model.ErrorMessage = error;
                await LoadLookupsAsync();
                return View(model);
            }

            SetSuccess("Đã cập nhật phiếu chi.");
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _api.DeleteExpenseAsync(id);
            if (!ok)
                SetError(error ?? "Xóa thất bại.");
            else
                SetSuccess("Đã xóa phiếu chi.");

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadLookupsAsync()
        {
            var categories = await _api.GetCategoriesAsync();
            var accounts = await _api.GetAccountsAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "Name");
            ViewBag.Accounts = new SelectList(
                accounts.Select(a => new { a.Id, Display = $"{a.Name} ({a.AccountNumber})" }),
                "Id", "Display");
        }
    }
}
