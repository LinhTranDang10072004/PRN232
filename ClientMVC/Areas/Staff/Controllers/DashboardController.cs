using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Staff.Controllers
{
    public class DashboardController : StaffBaseController
    {
        private readonly IStaffApiClient _api;

        public DashboardController(IStaffApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var dashboard = await _api.GetDashboardAsync() ?? new Models.Staff.StaffDashboardDto();
            ViewBag.UnreadCount = await _api.GetUnreadCountAsync();
            return View(dashboard);
        }
    }
}
