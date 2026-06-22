using ClientMVC.Helpers;
using ClientMVC.Models.Corporate;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Corporate.Controllers
{
    [Area("Corporate")]
    [Authorize(Roles = "Staff")]
    public class StaffController : Controller
    {
        private readonly ICorporateApiClient _api;

        public StaffController(ICorporateApiClient api) => _api = api;

        public async Task<IActionResult> Dashboard()
        {
            var y = DateTime.Today.Year;
            var m = DateTime.Today.Month;
            var (items, _) = await _api.GetStaffExpensesAsync(CalendarHelper.BuildMonthODataFilter(y, m));
            var list = items ?? new();
            var vm = new StaffDashboardViewModel
            {
                PendingCount = list.Count(x => x.Status == "Pending"),
                ApprovedCount = list.Count(x => x.Status == "Approved"),
                RejectedCount = list.Count(x => x.Status == "Rejected")
            };
            return View(vm);
        }
    }
}
