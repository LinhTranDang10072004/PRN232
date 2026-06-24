using ExpenseManagementAPI.DTOs.Staff;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IStaffExpenseService
    {
        IQueryable<StaffExpenseResponse> GetForStaff(int staffId);
        Task<(bool Success, StaffExpenseResponse? Data, string? Error)> GetByIdAsync(int staffId, int id);
        Task<(bool Success, StaffExpenseResponse? Data, string? Error)> CreateAsync(
            int staffId, int companyId, StaffExpenseRequest request);
        Task<(bool Success, StaffExpenseResponse? Data, string? Error)> UpdateAsync(
            int staffId, int companyId, int id, StaffExpenseRequest request);
        Task<(bool Success, string? Error)> DeleteAsync(int staffId, int id);
        Task<StaffDashboardResponse> GetDashboardAsync(int staffId);
    }
}
