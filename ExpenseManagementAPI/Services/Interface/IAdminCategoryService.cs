using ExpenseManagementAPI.DTOs.Admin;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IAdminCategoryService
    {
        IQueryable<AdminCategoryResponse> GetForCompany(int companyId);
        Task<(bool Success, AdminCategoryResponse? Data, string? Error)> CreateAsync(int companyId, AdminCategoryRequest request);
        Task<(bool Success, AdminCategoryResponse? Data, string? Error)> UpdateAsync(int companyId, int id, AdminCategoryRequest request);
        Task<(bool Success, string? Error)> DeleteAsync(int companyId, int id);
    }
}
