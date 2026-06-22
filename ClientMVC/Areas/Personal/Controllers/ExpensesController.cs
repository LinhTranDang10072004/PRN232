using ClientMVC.Helpers;
using ClientMVC.Models.Personal;
using ClientMVC.Models.Shared;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    [Area("Personal")]
    [Authorize(Roles = "User")]
    public class ExpensesController : Controller
    {
        private readonly IPersonalApiClient _api;

        public ExpensesController(IPersonalApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? filterKeyword,
            decimal? filterMinAmount,
            int? filterCategoryId,
            int? editId)
        {
            var vm = new ExpenseIndexViewModel
            {
                FilterKeyword = filterKeyword,
                FilterMinAmount = filterMinAmount,
                FilterCategoryId = filterCategoryId
            };

            var (categories, catErr) = await _api.GetCategoriesAsync("?$orderby=Name");
            if (catErr != null)
                vm.ErrorMessage = catErr;
            else
                vm.Categories = categories ?? new();

            var (items, err) = await _api.GetExpensesAsync(BuildExpenseODataQuery(vm));
            if (err != null)
                vm.ErrorMessage = err;
            else
                vm.Items = items ?? new();

            if (editId.HasValue)
            {
                var (item, editErr) = await _api.GetExpenseAsync(editId.Value);
                if (editErr != null)
                    vm.ErrorMessage = editErr;
                else if (item != null)
                {
                    vm.IsEdit = true;
                    vm.ShowFormModal = true;
                    vm.Form = new ExpenseFormModel
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = item.Description,
                        Amount = item.Amount,
                        ExpenseDate = item.ExpenseDate.Date,
                        CategoryId = item.CategoryId
                    };
                }
            }

            if (TempData["Success"] is string ok)
                vm.SuccessMessage = ok;
            if (TempData["Error"] is string fail)
                vm.ErrorMessage = fail;

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> Calendar(
            int? year, int? month, int? day, int? editId,
            string? filterKeyword, int? filterCategoryId, decimal? filterMinAmount)
        {
            var vm = await BuildPersonalCalendarAsync(year, month, day, new CalendarFilterModel
            {
                FilterKeyword = filterKeyword,
                FilterCategoryId = filterCategoryId,
                FilterMinAmount = filterMinAmount
            });

            if (editId.HasValue)
            {
                var (item, err) = await _api.GetExpenseAsync(editId.Value);
                if (err != null) vm.ErrorMessage = err;
                else if (item != null)
                {
                    vm.IsEdit = true;
                    vm.ShowFormModal = true;
                    vm.Form = new ExpenseFormModel
                    {
                        Id = item.Id,
                        Title = item.Title,
                        Description = item.Description,
                        Amount = item.Amount,
                        ExpenseDate = item.ExpenseDate.Date,
                        CategoryId = item.CategoryId
                    };
                }
            }

            if (TempData["Success"] is string ok) vm.SuccessMessage = ok;
            if (TempData["Error"] is string fail) vm.ErrorMessage = fail;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCalendar(
            [Bind(Prefix = "Form")] ExpenseFormModel form,
            int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, decimal? filterMinAmount)
        {
            if (!ModelState.IsValid)
                return View("Calendar", await BuildPersonalCalendarAsync(year, month, day,
                    BuildFilter(filterKeyword, filterCategoryId, null, filterMinAmount), form, showModal: true));

            var (_, error) = await _api.CreateExpenseAsync(form);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã thêm khoản chi.";
            var dt = form.ExpenseDate.Date;
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(dt.Year, dt.Month, dt.Day,
                BuildFilter(filterKeyword, filterCategoryId, null, filterMinAmount)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCalendar(
            int id, [Bind(Prefix = "Form")] ExpenseFormModel form,
            int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, decimal? filterMinAmount)
        {
            if (!ModelState.IsValid)
                return View("Calendar", await BuildPersonalCalendarAsync(year, month, day,
                    BuildFilter(filterKeyword, filterCategoryId, null, filterMinAmount), form, showModal: true, isEdit: true));

            var (_, error) = await _api.UpdateExpenseAsync(id, form);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã cập nhật.";
            var dt = form.ExpenseDate.Date;
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(dt.Year, dt.Month, dt.Day,
                BuildFilter(filterKeyword, filterCategoryId, null, filterMinAmount)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCalendar(
            int id, int year, int month, int day,
            string? filterKeyword, int? filterCategoryId, decimal? filterMinAmount)
        {
            var (_, error) = await _api.DeleteExpenseAsync(id);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã xóa.";
            return RedirectToAction(nameof(Calendar), CalendarHelper.CalendarRoute(year, month, day,
                BuildFilter(filterKeyword, filterCategoryId, null, filterMinAmount)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "Form")] ExpenseFormModel form)
        {
            if (!ModelState.IsValid)
                return await IndexWithFormAsync(form, isEdit: false);

            var (_, error) = await _api.CreateExpenseAsync(form);
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Đã thêm khoản chi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind(Prefix = "Form")] ExpenseFormModel form)
        {
            if (!ModelState.IsValid)
                return await IndexWithFormAsync(form, isEdit: true);

            var (_, error) = await _api.UpdateExpenseAsync(id, form);
            if (error != null)
            {
                TempData["Error"] = error;
                return RedirectToAction(nameof(Index));
            }

            TempData["Success"] = "Đã cập nhật khoản chi.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (_, error) = await _api.DeleteExpenseAsync(id);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã xóa khoản chi.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> IndexWithFormAsync(ExpenseFormModel form, bool isEdit)
        {
            var vm = new ExpenseIndexViewModel
            {
                Form = form,
                ShowFormModal = true,
                IsEdit = isEdit
            };

            var (categories, _) = await _api.GetCategoriesAsync("?$orderby=Name");
            vm.Categories = categories ?? new();

            var (items, err) = await _api.GetExpensesAsync("?$orderby=ExpenseDate desc");
            vm.Items = items ?? new();
            if (err != null)
                vm.ErrorMessage = err;

            return View("Index", vm);
        }

        private static string BuildExpenseODataQuery(ExpenseIndexViewModel vm)
        {
            var filters = new List<string>();
            if (!string.IsNullOrWhiteSpace(vm.FilterKeyword))
            {
                var kw = vm.FilterKeyword.Trim().Replace("'", "''");
                filters.Add($"contains(Title,'{kw}')");
            }
            if (vm.FilterMinAmount.HasValue)
                filters.Add($"Amount ge {vm.FilterMinAmount.Value}");
            if (vm.FilterCategoryId.HasValue)
                filters.Add($"CategoryId eq {vm.FilterCategoryId.Value}");

            var query = "?$orderby=ExpenseDate desc&$top=100";
            if (filters.Count > 0)
                query += "&$filter=" + string.Join(" and ", filters);
            return query;
        }

        private async Task<PersonalCalendarViewModel> BuildPersonalCalendarAsync(
            int? year, int? month, int? day, CalendarFilterModel filters,
            ExpenseFormModel? form = null, bool showModal = false, bool isEdit = false)
        {
            var y = year ?? DateTime.Today.Year;
            var m = month ?? DateTime.Today.Month;
            var d = day ?? DateTime.Today.Day;
            if (d > DateTime.DaysInMonth(y, m)) d = 1;

            var vm = new PersonalCalendarViewModel
            {
                Year = y,
                Month = m,
                SelectedDay = d,
                Filters = filters,
                Form = form ?? new ExpenseFormModel { ExpenseDate = new DateTime(y, m, d) },
                ShowFormModal = showModal,
                IsEdit = isEdit
            };

            var (categories, catErr) = await _api.GetCategoriesAsync("?$orderby=Name");
            if (catErr != null) vm.ErrorMessage = catErr;
            else vm.Categories = categories ?? new();

            var (items, err) = await _api.GetExpensesAsync(CalendarHelper.BuildMonthODataFilter(y, m, filters));
            if (err != null) vm.ErrorMessage = err;
            else
            {
                vm.MonthExpenses = items ?? new();
                vm.CalendarDays = CalendarHelper.BuildPersonalMonthGrid(y, m, vm.MonthExpenses, d);
                vm.DayExpenses = vm.MonthExpenses.Where(e => e.ExpenseDate.Day == d).OrderByDescending(e => e.Amount).ToList();
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
