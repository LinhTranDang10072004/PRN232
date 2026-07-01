using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class MonthClosingController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public MonthClosingController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index(int? month, int? year)
        {
            var m = month ?? DateTime.Today.Month;
            var y = year ?? DateTime.Today.Year;
            var preview = await _api.GetMonthClosingPreviewAsync(m, y);
            if (preview == null)
            {
                SetError("Không tải được dữ liệu chốt sổ.");
                preview = new Models.Personal.MonthClosingPreviewDto { Month = m, Year = y };
            }

            ViewBag.Month = m;
            ViewBag.Year = y;
            return View(preview);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Close(int month, int year, string? notes)
        {
            var (ok, _, error) = await _api.CloseMonthAsync(month, year, notes);
            if (!ok)
            {
                SetError(error ?? "Chốt sổ thất bại.");
                return RedirectToAction(nameof(Index), new { month, year });
            }

            SetSuccess($"Đã chốt sổ tháng {month}/{year}. Số dư còn lại vẫn nằm trong ví của bạn.");
            return RedirectToAction(nameof(Index), new { month, year });
        }
    }
}
