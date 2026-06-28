using ClientMVC.Models.Admin;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Admin.Controllers
{
    public class CategoriesController : AdminBaseController
    {
        private readonly IAdminApiClient _api;

        public CategoriesController(IAdminApiClient api) => _api = api;

        public async Task<IActionResult> Index()
        {
            var items = await _api.GetCategoriesAsync();
            return View(items.OrderBy(c => c.Name).ToList());
        }

        public IActionResult Create() => View(new AdminCategoryFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AdminCategoryFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, error) = await _api.CreateCategoryAsync(model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Thêm thất bại.");
                return View(model);
            }
            SetSuccess("Đã thêm danh mục.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var items = await _api.GetCategoriesAsync();
            var item = items.FirstOrDefault(c => c.Id == id);
            if (item == null) return NotFound();
            return View(new AdminCategoryFormModel
            {
                Id = item.Id,
                Name = item.Name,
                Status = item.Status
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AdminCategoryFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, error) = await _api.UpdateCategoryAsync(id, model);
            if (!ok)
            {
                ModelState.AddModelError(string.Empty, error ?? "Cập nhật thất bại.");
                return View(model);
            }
            SetSuccess("Đã cập nhật danh mục.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (ok, error) = await _api.DeleteCategoryAsync(id);
            if (!ok) SetError(error ?? "Xóa thất bại.");
            else SetSuccess("Đã xóa danh mục.");
            return RedirectToAction(nameof(Index));
        }
    }
}
