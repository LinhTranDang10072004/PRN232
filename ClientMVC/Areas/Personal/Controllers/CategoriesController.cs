using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    public class CategoriesController : PersonalBaseController
    {
        private readonly IPersonalApiClient _api;

        public CategoriesController(IPersonalApiClient api) => _api = api;

        public async Task<IActionResult> Index() => View(await _api.GetCategoriesAsync());

        public IActionResult Create() => View(new CategoryFormModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, _, error) = await _api.CreateCategoryAsync(model);
            if (!ok) { model.ErrorMessage = error; return View(model); }
            SetSuccess("Đã thêm danh mục.");
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var categories = await _api.GetCategoriesAsync();
            var cat = categories.FirstOrDefault(c => c.Id == id);
            if (cat == null || !cat.CanEdit) return NotFound();
            return View(new CategoryFormModel { Id = id, Name = cat.Name });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var (ok, _, error) = await _api.UpdateCategoryAsync(id, model);
            if (!ok) { model.ErrorMessage = error; return View(model); }
            SetSuccess("Đã cập nhật danh mục.");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deactivate(int id)
        {
            var (ok, error) = await _api.DeactivateCategoryAsync(id);
            if (!ok) SetError(error ?? "Thất bại.");
            else SetSuccess("Đã xóa/inactive danh mục.");
            return RedirectToAction(nameof(Index));
        }
    }
}
