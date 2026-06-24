using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class ReportsController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public ReportsController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index(int? month, int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;
            ViewBag.Month = m;
            ViewBag.Year = y;
            ViewBag.Summary = await _api.GetMonthlySummaryAsync(m, y);
            ViewBag.ByCategory = await _api.GetCategoryReportAsync(m, y);
            ViewBag.BudgetStatus = await _api.GetBudgetStatusAsync(m, y);
            ViewBag.Yearly = await _api.GetYearlyReportAsync(y);
            return View();
        }
    }
}
