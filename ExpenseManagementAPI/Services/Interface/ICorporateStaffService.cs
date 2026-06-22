using ExpenseManagementAPI.DTOs.Corporate;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface ICorporateStaffService
    {
        Task<List<StaffResponse>> GetStaffListAsync(int adminId);
        Task<(StaffResponse? Result, string? Error)> CreateStaffAsync(int adminId, CreateStaffRequest request);
        Task<(StaffResponse? Result, string? Error)> SetStaffActiveAsync(int adminId, int staffId, bool isActive);
    }
}
