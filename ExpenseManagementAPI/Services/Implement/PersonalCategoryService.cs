using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalCategoryService : IPersonalCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public PersonalCategoryService(ICategoryRepository categoryRepository) =>
            _categoryRepository = categoryRepository;

        public IQueryable<CategoryResponse> GetForUser(int userId) =>
            _categoryRepository.GetForPersonalUserManage(userId).Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status,
                IsSystem = c.OwnerUserId == null,
                CanEdit = c.OwnerUserId == userId,
                CanDelete = c.OwnerUserId == userId
            });

        public async Task<(bool Success, CategoryResponse? Data, string? Error)> GetByIdAsync(int userId, int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || !IsAccessible(category, userId))
                return (false, null, "Không tìm thấy danh mục.");

            return (true, Map(category, userId), null);
        }

        public async Task<(bool Success, CategoryResponse? Data, string? Error)> CreateAsync(
            int userId, CategoryRequest request)
        {
            if (await _categoryRepository.NameExistsForPersonalUserAsync(userId, request.Name))
                return (false, null, "Tên danh mục đã tồn tại.");

            var category = new Category
            {
                Name = request.Name.Trim(),
                OwnerUserId = userId,
                CompanyId = null,
                Type = "Expense",
                Status = CategoryStatus.Active
            };

            await _categoryRepository.AddAsync(category);
            return (true, Map(category, userId), null);
        }

        public async Task<(bool Success, CategoryResponse? Data, string? Error)> UpdateAsync(
            int userId, int id, CategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || !CategoryScopeRules.CanModifyPersonalCategory(category, userId))
                return (false, null, "Không thể sửa danh mục này.");

            if (await _categoryRepository.NameExistsForPersonalUserAsync(userId, request.Name, id))
                return (false, null, "Tên danh mục đã tồn tại.");

            category.Name = request.Name.Trim();
            await _categoryRepository.UpdateAsync(category);
            return (true, Map(category, userId), null);
        }

        public async Task<(bool Success, string? Error)> DeactivateAsync(int userId, int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || !CategoryScopeRules.CanModifyPersonalCategory(category, userId))
                return (false, "Không thể xóa danh mục này.");

            if (await _categoryRepository.IsUsedByExpensesAsync(id))
            {
                category.Status = CategoryStatus.Inactive;
                await _categoryRepository.UpdateAsync(category);
                return (true, null);
            }

            await _categoryRepository.DeleteAsync(category);
            return (true, null);
        }

        private static bool IsAccessible(Category category, int userId) =>
            category.CompanyId == null &&
            ((category.OwnerUserId == null && category.Status == CategoryStatus.Active) ||
             category.OwnerUserId == userId);

        private static CategoryResponse Map(Category c, int userId) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Status = c.Status,
            IsSystem = c.OwnerUserId == null,
            CanEdit = CategoryScopeRules.CanModifyPersonalCategory(c, userId),
            CanDelete = CategoryScopeRules.CanModifyPersonalCategory(c, userId)
        };
    }
}
