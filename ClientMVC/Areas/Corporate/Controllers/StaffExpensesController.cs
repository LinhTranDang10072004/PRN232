using ClientMVC.Helpers;
using ClientMVC.Models.Corporate;
using ClientMVC.Models.Shared;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Corporate.Controllers
{
    [Area("Corporate")]
    [Authorize(Roles = "Staff")]
    public class StaffExpensesController : Controller
    {
        private readonly ICorporateApiClient _api;

        public StaffExpensesController(ICorporateApiClient api) => _api = api;

        [HttpGet]
        public async Task<IActionResult> Calendar(
            int? year, int? month, int? day, int? editId,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            var vm = await BuildCalendarAsync(year, month, day, new CalendarFilterModel
            {
                FilterKeyword = filterKeyword,
                FilterCategoryId = filterCategoryId,
                FilterStatus = filterStatus,
                FilterMinAmount = filterMinAmount
            });

            if (editId.HasValue)
                await LoadEditFormAsync(vm, editId.Value);

            ApplyTempData(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind(Prefix = "Form")] CorporateExpenseFormModel form,
            int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            if (!ModelState.IsValid)
                return View("Calendar", await BuildCalendarAsync(year, month, day, BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount), form, showModal: true));

            var (_, error) = await _api.CreateStaffExpenseAsync(form);
            if (error != null)
            {
                TempData["Error"] = error;
            }
            else
            {
                TempData["Success"] = "Đã gửi khoản chi chờ Admin duyệt.";
            }

            var dt = form.ExpenseDate.Date;
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(dt.Year, dt.Month, dt.Day,
                BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id, [Bind(Prefix = "Form")] CorporateExpenseFormModel form,
            int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            if (!ModelState.IsValid)
                return View("Calendar", await BuildCalendarAsync(year, month, day, BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount), form, showModal: true, isEdit: true));

            var (_, error) = await _api.UpdateStaffExpenseAsync(id, form);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã cập nhật.";

            var dt = form.ExpenseDate.Date;
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(dt.Year, dt.Month, dt.Day,
                BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(
            int id, int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, string? filterStatus, decimal? filterMinAmount)
        {
            var (_, error) = await _api.DeleteStaffExpenseAsync(id);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã xóa.";
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(year, month, day,
                BuildFilter(filterKeyword, filterCategoryId, filterStatus, filterMinAmount)));
        }

        private async Task<CorporateCalendarViewModel> BuildCalendarAsync(
            int? year, int? month, int? day, CalendarFilterModel filters,
            CorporateExpenseFormModel? form = null, bool showModal = false, bool isEdit = false)
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
                IsAdminView = false,
                Form = form ?? new CorporateExpenseFormModel { ExpenseDate = new DateTime(y, m, d) },
                ShowFormModal = showModal,
                IsEdit = isEdit
            };

            var (categories, catErr) = await _api.GetStaffCategoriesAsync();
            if (catErr != null) vm.ErrorMessage = catErr;
            else vm.Categories = categories ?? new();

            var (items, err) = await _api.GetStaffExpensesAsync(CalendarHelper.BuildMonthODataFilter(y, m, filters));
            if (err != null) vm.ErrorMessage = err;
            else
            {
                vm.MonthExpenses = items ?? new();
                vm.PendingCount = vm.MonthExpenses.Count(x => x.Status == "Pending");
                vm.CalendarDays = CalendarHelper.BuildMonthGrid(y, m, vm.MonthExpenses, d);
                vm.DayExpenses = vm.MonthExpenses
                    .Where(e => e.ExpenseDate.Day == d)
                    .OrderByDescending(e => e.Amount)
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

        private async Task LoadEditFormAsync(CorporateCalendarViewModel vm, int editId)
        {
            var (item, err) = await _api.GetStaffExpenseAsync(editId);
            if (err != null) { vm.ErrorMessage = err; return; }
            if (item == null) { vm.ErrorMessage = "Không tìm thấy."; return; }
            if (!item.CanEdit) { vm.ErrorMessage = "Khoản chi đã duyệt — không thể sửa."; return; }

            vm.IsEdit = true;
            vm.ShowFormModal = true;
            vm.Form = new CorporateExpenseFormModel
            {
                Id = item.Id,
                Title = item.Title,
                Description = item.Description,
                Amount = item.Amount,
                ExpenseDate = item.ExpenseDate.Date,
                CategoryId = item.CategoryId
            };
        }

        private void ApplyTempData(CorporateCalendarViewModel vm)
        {
            if (TempData["Success"] is string ok) vm.SuccessMessage = ok;
            if (TempData["Error"] is string fail) vm.ErrorMessage = fail;
        }
    }
}
