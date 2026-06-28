using ExpenseManagementAPI.DTOs.Admin;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IAdminStaffService
    {
        Task<List<StaffUserResponse>> GetStaffAsync(int companyId);
        Task<(bool Success, StaffUserResponse? Data, string? Error)> CreateStaffAsync(int companyId, CreateStaffRequest request);
        Task<(bool Success, string? Error)> UpdateStaffStatusAsync(int companyId, int staffId, bool isActive);
    }
}
