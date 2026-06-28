using BusinessObjects.Enums;
using ClientMVC.Models.Admin;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Admin.Controllers
{
    public class ExpensesController : AdminBaseController
    {
        private readonly IAdminApiClient _api;

        public ExpensesController(IAdminApiClient api) => _api = api;

        public async Task<IActionResult> Index(string? status)
        {
            string? filter = status?.ToLowerInvariant() switch
            {
                "pending" => "Status eq 1",
                "approved" => "Status eq 2",
                "rejected" => "Status eq 3",
                _ => null
            };

            var expenses = await _api.GetExpensesAsync(filter);
            ViewBag.Status = status;
            return View(expenses.OrderByDescending(e => e.CreatedAt).ToList());
        }

        public async Task<IActionResult> Details(int id)
        {
            var expense = await _api.GetExpenseAsync(id);
            if (expense == null) return NotFound();
            return View(expense);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            var (ok, error) = await _api.ApproveExpenseAsync(id);
            if (!ok) SetError(error ?? "Duyệt thất bại.");
            else SetSuccess("Đã duyệt phiếu chi.");
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, RejectExpenseFormModel model)
        {
            if (!ModelState.IsValid)
            {
                SetError("Nhập lý do từ chối.");
                return RedirectToAction(nameof(Details), new { id });
            }

            var (ok, error) = await _api.RejectExpenseAsync(id, model.Comment);
            if (!ok) SetError(error ?? "Từ chối thất bại.");
            else SetSuccess("Đã từ chối phiếu chi.");
            return RedirectToAction(nameof(Details), new { id });
        }
    }
}
