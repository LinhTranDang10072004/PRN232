using BusinessObjects.Enums;
using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services
{
    public class PersonalCategoryService : IPersonalCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public PersonalCategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public IQueryable<CategoryResponse> GetMyCategories(int userId) =>
            _categoryRepository.GetForPersonalUser(userId).Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Branch = "Personal",
                IsSystem = c.OwnerUserId == null,
                OwnerUserId = c.OwnerUserId
            });

        public async Task<CategoryResponse?> GetByIdAsync(int userId, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (!IsAccessible(category, userId))
                return null;

            return MapToResponse(category!);
        }

        public async Task<(CategoryResponse? Result, string? Error)> CreateAsync(
            int userId, CreateCategoryRequest request)
        {
            var name = request.Name.Trim();
            if (await _categoryRepository.NameExistsForUserAsync(userId, name))
                return (null, "Bạn đã có danh mục trùng tên.");

            var category = new Category
            {
                Name = name,
                Branch = CategoryBranch.Personal,
                OwnerUserId = userId
            };

            await _categoryRepository.AddAsync(category);
            var created = await _categoryRepository.GetByIdAsync(category.Id);
            return (created == null ? null : MapToResponse(created), null);
        }

        public async Task<(CategoryResponse? Result, string? Error)> UpdateAsync(
            int userId, int categoryId, UpdateCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (!IsOwnedByUser(category, userId))
                return (null, category == null
                    ? "Không tìm thấy danh mục."
                    : "Không thể sửa danh mục hệ thống.");

            var name = request.Name.Trim();
            if (await _categoryRepository.NameExistsForUserAsync(userId, name, categoryId))
                return (null, "Bạn đã có danh mục trùng tên.");

            category!.Name = name;
            await _categoryRepository.UpdateAsync(category);
            var updated = await _categoryRepository.GetByIdAsync(categoryId);
            return (updated == null ? null : MapToResponse(updated), null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int userId, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (!IsOwnedByUser(category, userId))
                return (false, category == null
                    ? "Không tìm thấy danh mục."
                    : "Không thể xóa danh mục hệ thống.");

            if (await _categoryRepository.IsUsedByExpensesAsync(categoryId))
                return (false, "Danh mục đang được dùng trong chi tiêu, không thể xóa.");

            await _categoryRepository.DeleteAsync(category!);
            return (true, null);
        }

        public async Task<bool> CanUserUseCategoryAsync(int userId, int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            return IsAccessible(category, userId);
        }

        private static bool IsAccessible(Category? category, int userId) =>
            category != null &&
            category.Branch == CategoryBranch.Personal &&
            (category.OwnerUserId == null || category.OwnerUserId == userId);

        private static bool IsOwnedByUser(Category? category, int userId) =>
            category != null &&
            category.Branch == CategoryBranch.Personal &&
            category.OwnerUserId == userId;

        private static CategoryResponse MapToResponse(Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Branch = c.Branch.ToString(),
            IsSystem = c.OwnerUserId == null,
            OwnerUserId = c.OwnerUserId
        };
    }
}
