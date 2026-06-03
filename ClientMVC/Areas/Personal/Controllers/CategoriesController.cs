using ClientMVC.Models.Personal;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    [Area("Personal")]
    [Authorize(Roles = "User")]
    public class CategoriesController : Controller
    {
        private readonly IPersonalApiClient _api;

        public CategoriesController(IPersonalApiClient api)
        {
            _api = api;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? filterKeyword, int? editId)
        {
            var vm = new CategoryIndexViewModel { FilterKeyword = filterKeyword };

            var (items, err) = await _api.GetCategoriesAsync(BuildCategoryODataQuery(vm));
            if (err != null)
                vm.ErrorMessage = err;
            else
                vm.Items = items ?? new();

            if (editId.HasValue)
            {
                var (item, editErr) = await _api.GetCategoryAsync(editId.Value);
                if (editErr != null)
                    vm.ErrorMessage = editErr;
                else if (item != null && !item.IsSystem)
                {
                    vm.IsEdit = true;
                    vm.ShowFormModal = true;
                    vm.Form = new CategoryFormModel { Id = item.Id, Name = item.Name };
                }
                else if (item?.IsSystem == true)
                    vm.ErrorMessage = "Không thể sửa danh mục hệ thống.";
            }

            if (TempData["Success"] is string ok)
                vm.SuccessMessage = ok;
            if (TempData["Error"] is string fail)
                vm.ErrorMessage = fail;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormModel form)
        {
            if (!ModelState.IsValid)
                return await IndexWithFormAsync(form, isEdit: false);

            var (_, error) = await _api.CreateCategoryAsync(form);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã thêm danh mục.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryFormModel form)
        {
            if (!ModelState.IsValid)
                return await IndexWithFormAsync(form, isEdit: true);

            var (_, error) = await _api.UpdateCategoryAsync(id, form);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã cập nhật danh mục.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (_, error) = await _api.DeleteCategoryAsync(id);
            TempData[error == null ? "Success" : "Error"] = error ?? "Đã xóa danh mục.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<IActionResult> IndexWithFormAsync(CategoryFormModel form, bool isEdit)
        {
            var vm = new CategoryIndexViewModel
            {
                Form = form,
                ShowFormModal = true,
                IsEdit = isEdit
            };

            var (items, err) = await _api.GetCategoriesAsync("?$orderby=Name");
            vm.Items = items ?? new();
            if (err != null)
                vm.ErrorMessage = err;

            return View("Index", vm);
        }

        private static string BuildCategoryODataQuery(CategoryIndexViewModel vm)
        {
            var query = "?$orderby=Name&$top=100";
            if (!string.IsNullOrWhiteSpace(vm.FilterKeyword))
            {
                var kw = vm.FilterKeyword.Trim().Replace("'", "''");
                query += $"&$filter=contains(Name,'{kw}')";
            }
            return query;
        }
    }
}
