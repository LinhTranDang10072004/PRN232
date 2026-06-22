using ExpenseManagementAPI.DTOs.Corporate;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface ICorporateExpenseService
    {
        IQueryable<CorporateExpenseResponse> GetForStaff(int staffId);
        IQueryable<CorporateExpenseResponse> GetForAdmin(int adminId);
        Task<CorporateExpenseResponse?> GetByIdForStaffAsync(int staffId, int expenseId);
        Task<CorporateExpenseResponse?> GetByIdForAdminAsync(int adminId, int expenseId);
        Task<(CorporateExpenseResponse? Result, string? Error)> CreateForStaffAsync(int staffId, CorporateExpenseRequest request);
        Task<(CorporateExpenseResponse? Result, string? Error)> UpdateForStaffAsync(int staffId, int expenseId, CorporateExpenseRequest request);
        Task<(bool Success, string? Error)> DeleteForStaffAsync(int staffId, int expenseId);
        Task<(bool Success, string? Error)> ApproveAsync(int adminId, int expenseId);
        Task<(bool Success, string? Error)> RejectAsync(int adminId, int expenseId, string reason);
        Task<List<CategoryOptionDto>> GetCorporateCategoriesAsync();
    }
}
