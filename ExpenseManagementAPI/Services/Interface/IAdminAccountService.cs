using ExpenseManagementAPI.DTOs.Admin;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IAdminAccountService
    {
        IQueryable<AdminAccountResponse> GetForCompany(int companyId);
        Task<(bool Success, AdminAccountResponse? Data, string? Error)> CreateAsync(int companyId, AdminAccountRequest request);
        Task<(bool Success, AdminAccountResponse? Data, string? Error)> UpdateAsync(int companyId, int id, AdminAccountRequest request);
    }
}
