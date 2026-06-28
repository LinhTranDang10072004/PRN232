using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class AdminCategoryService : IAdminCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public AdminCategoryService(ICategoryRepository categoryRepository) =>
            _categoryRepository = categoryRepository;

        public IQueryable<AdminCategoryResponse> GetForCompany(int companyId) =>
            _categoryRepository.GetForCorporateCompanyManage(companyId).Select(c => new AdminCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status
            });

        public async Task<(bool Success, AdminCategoryResponse? Data, string? Error)> CreateAsync(
            int companyId, AdminCategoryRequest request)
        {
            var (ok, error) = CategoryScopeRules.ValidateCorporateCreate(companyId, companyId, null);
            if (!ok)
                return (false, null, error);

            var name = request.Name.Trim();
            if (await _categoryRepository.NameExistsForCompanyAsync(companyId, name))
                return (false, null, "Danh mục trùng tên trong công ty.");

            var category = new Category
            {
                Name = name,
                CompanyId = companyId,
                OwnerUserId = null,
                Status = request.Status
            };

            await _categoryRepository.AddAsync(category);
            return (true, Map(category), null);
        }

        public async Task<(bool Success, AdminCategoryResponse? Data, string? Error)> UpdateAsync(
            int companyId, int id, AdminCategoryRequest request)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.CompanyId != companyId)
                return (false, null, "Không tìm thấy danh mục.");

            var name = request.Name.Trim();
            if (await _categoryRepository.NameExistsForCompanyAsync(companyId, name, id))
                return (false, null, "Danh mục trùng tên trong công ty.");

            category.Name = name;
            category.Status = request.Status;
            await _categoryRepository.UpdateAsync(category);
            return (true, Map(category), null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int companyId, int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null || category.CompanyId != companyId)
                return (false, "Không tìm thấy danh mục.");

            if (await _categoryRepository.IsUsedByExpensesAsync(id))
                return (false, "Danh mục đang được dùng trong phiếu chi.");

            await _categoryRepository.DeleteAsync(category);
            return (true, null);
        }

        private static AdminCategoryResponse Map(Category c) => new()
        {
            Id = c.Id,
            Name = c.Name,
            Status = c.Status
        };
    }
}
