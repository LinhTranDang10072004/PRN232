using BusinessObjects.Enums;
using ClientMVC.Helpers;
using ClientMVC.Models;
using ClientMVC.Models.Admin;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClientMVC.Areas.Admin.Controllers
{
    public class ExpensesController : AdminBaseController
    {
        private readonly IAdminApiClient _api;

        public ExpensesController(IAdminApiClient api) => _api = api;

        public async Task<IActionResult> Index(ExpenseFilterModel filter, string? status)
        {
            if (!filter.Status.HasValue && !string.IsNullOrEmpty(status))
            {
                filter.Status = status.ToLowerInvariant() switch
                {
                    "pending" => ExpenseStatus.Pending,
                    "approved" => ExpenseStatus.Approved,
                    "rejected" => ExpenseStatus.Rejected,
                    _ => null
                };
            }

            await LoadFilterLookupsAsync();
            var oDataFilter = ODataExpenseFilterBuilder.Build(filter, ExpenseFilterScope.Admin);
            var expenses = await _api.GetExpensesAsync(oDataFilter);
            SetFilterViewBag(filter, expenses);
            ViewBag.Status = status;
            return View(expenses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var expense = await _api.GetExpenseAsync(id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var (ok, error) = await _api.ApproveExpenseAsync(id);
            if (!ok) SetError(error ?? "Duyệt thất bại.");
            else SetSuccess("Đã duyệt phiếu chi.");
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, RejectExpenseFormModel model)
        {
            if (!ModelState.IsValid)
            {
                SetError("Nhập lý do từ chối.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var (ok, error) = await _api.RejectExpenseAsync(id, model.Comment);
            if (!ok) SetError(error ?? "Từ chối thất bại.");
            else SetSuccess("Đã từ chối phiếu chi.");
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task LoadFilterLookupsAsync()
        {
            var categories = await _api.GetCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Name");
        }

        private void SetFilterViewBag(ExpenseFilterModel filter, List<AdminExpenseDto> expenses)
        {
            ViewBag.Filter = filter;
            ViewBag.FilterConfig = new ExpenseFilterViewConfig
            {
                Area = "Admin",
                ShowStatus = true,
                ShowStaffSearch = true
            };
            ViewBag.ResultCount = expenses.Count;
            ViewBag.TotalAmount = expenses.Sum(e => e.Amount);
        }
    }
}
