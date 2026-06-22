using ClientMVC.Helpers;
using ClientMVC.Models.Corporate;
using ClientMVC.Models.Shared;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Corporate.Controllers
{
    [Area("Corporate")]
    [Authorize(Roles = "Admin")]
    public class AdminExpensesController : Controller
    {
        private readonly ICorporateApiClient _api;

        public AdminExpensesController(ICorporateApiClient api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Calendar(
            int? year, int? month, int? day, int? reviewId,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            var vm = await BuildCalendarAsync(year, month, day, new CalendarFilterModel
            {
                FilterKeyword = filterKeyword,
                FilterCategoryId = filterCategoryId,
                FilterStatus = filterStatus,
                FilterMinAmount = filterMinAmount
            });
            vm.ReviewExpenseId = reviewId;

            if (TempData["Success"] is string ok) vm.SuccessMessage = ok;
            if (TempData["Error"] is string fail) vm.ErrorMessage = fail;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(
            int id, int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            var (_, error) = await _api.ApproveExpenseAsync(id);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã chấp nhận khoản chi.";
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(year, month, day,
                BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(
            int id, string rejectReason, int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            if (string.IsNullOrWhiteSpace(rejectReason))
            {
                TempData["Error"] = "Nhập lý do từ chối.";
                return RedirectToAction(nameof(Calendar), new
                {
                    year, month, day, reviewId = id,
                    filterKeyword, filterCategoryId, filterStatus, filterMinAmount
                });
            }

            var (_, error) = await _api.RejectExpenseAsync(id, rejectReason);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã từ chối khoản chi.";
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(year, month, day,
                BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount)));
        }

        private async Task<CorporateCalendarViewModel> BuildCalendarAsync(
            int? year, int? month, int? day, CalendarFilterModel filters)
        {
            var y = year ?? DateTime.Today.Year;
            var m = month ?? DateTime.Today.Month;
            var d = day ?? DateTime.Today.Day;
            if (d > DateTime.DaysInMonth(y, m)) d = 1;

            var vm = new CorporateCalendarViewModel
            {
                Year = y,
                Month = m,
                SelectedDay = d,
                Filters = filters,
                IsAdminView = true
            };

            var (categories, _) = await _api.GetAdminCategoriesAsync();
            vm.Categories = categories ?? new();

            var (items, err) = await _api.GetAdminExpensesAsync(CalendarHelper.BuildMonthODataFilter(y, m, filters));
            if (err != null) vm.ErrorMessage = err;
            else
            {
                vm.MonthExpenses = items ?? new();
                vm.PendingCount = vm.MonthExpenses.Count(x => x.Status == "Pending");
                vm.CalendarDays = CalendarHelper.BuildMonthGrid(y, m, vm.MonthExpenses, d);
                vm.DayExpenses = vm.MonthExpenses
                    .Where(e => e.ExpenseDate.Day == d)
                    .OrderBy(e => e.Status == "Pending" ? 0 : 1)
                    .ThenByDescending(e => e.Amount)
                    .ToList();
            }

            return vm;
        }

        private static CalendarFilterModel BuildFilter(
            string? keyword, int? categoryId, string? status, decimal? minAmount) => new()
        {
            FilterKeyword = keyword,
            FilterCategoryId = categoryId,
            FilterStatus = status,
            FilterMinAmount = minAmount
        };
    }
}
