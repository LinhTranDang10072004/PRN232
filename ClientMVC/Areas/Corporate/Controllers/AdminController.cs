using ClientMVC.Helpers;
using ClientMVC.Models.Corporate;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Corporate.Controllers
{
    [Area("Corporate")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICorporateApiClient _api;

        public AdminController(ICorporateApiClient api) => _api = api;

        public async Task<IActionResult> Dashboard()
        {
            var y = DateTime.Today.Year;
            var m = DateTime.Today.Month;
            var (staffs, _) = await _api.GetStaffsAsync();
            var (expenses, _) = await _api.GetAdminExpensesAsync(CalendarHelper.BuildMonthODataFilter(y, m));
            var list = expenses ?? new();

            var vm = new AdminDashboardViewModel
            {
                StaffCount = staffs?.Count ?? 0,
                PendingCount = list.Count(x => x.Status == "Pending"),
                MonthExpenseCount = list.Count,
                MonthTotalAmount = list.Sum(x => x.Amount)
            };
            return View(vm);
        }
    }
}
