using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Admin.Controllers
{
    public class DashboardController : AdminBaseController
    {
        private readonly IAdminApiClient _api;

        public DashboardController(IAdminApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var dashboard = await _api.GetDashboardAsync() ?? new Models.Admin.AdminDashboardDto();
            return View(dashboard);
        }
    }
}
