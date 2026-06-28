using ExpenseManagementAPI.DTOs.Admin;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IAdminExpenseService
    {
        IQueryable<AdminExpenseResponse> GetForCompany(int companyId);
        Task<(bool Success, AdminExpenseResponse? Data, string? Error)> GetByIdAsync(int companyId, int id);
        Task<(bool Success, string? Error)> ApproveAsync(int adminId, int companyId, int expenseId);
        Task<(bool Success, string? Error)> RejectAsync(int adminId, int companyId, int expenseId, string comment);
        Task<AdminDashboardResponse> GetDashboardAsync(int companyId);
    }
}
