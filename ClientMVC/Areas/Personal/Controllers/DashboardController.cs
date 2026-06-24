using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class DashboardController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public DashboardController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Today;
            var summary = await _api.GetMonthlySummaryAsync(now.Month, now.Year);
            var budgets = await _api.GetBudgetStatusAsync(now.Month, now.Year);
            var categories = await _api.GetCategoryReportAsync(now.Month, now.Year);
            var unread = await _api.GetUnreadCountAsync();

            return View(new DashboardViewModel
            {
                Summary = summary,
                Budgets = budgets.Take(5).ToList(),
                TopCategories = categories.Take(5).ToList(),
                UnreadNotifications = unread
            });
        }
    }
}
