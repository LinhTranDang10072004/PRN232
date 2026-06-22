using ClientMVC.Models.Corporate;

namespace ClientMVC.Services
{
    public interface ICorporateApiClient
    {
        Task<(List<CorporateExpenseItem>? Data, string? Error)> GetStaffExpensesAsync(string? odataQuery = null);
        Task<(CorporateExpenseItem? Data, string? Error)> GetStaffExpenseAsync(int id);
        Task<(List<CategoryOptionItem>? Data, string? Error)> GetStaffCategoriesAsync();
        Task<(CorporateExpenseItem? Data, string? Error)> CreateStaffExpenseAsync(CorporateExpenseFormModel model);
        Task<(CorporateExpenseItem? Data, string? Error)> UpdateStaffExpenseAsync(int id, CorporateExpenseFormModel model);
        Task<(bool Success, string? Error)> DeleteStaffExpenseAsync(int id);

        Task<(List<CategoryOptionItem>? Data, string? Error)> GetAdminCategoriesAsync();
        Task<(List<CorporateExpenseItem>? Data, string? Error)> GetAdminExpensesAsync(string? odataQuery = null);
        Task<(bool Success, string? Error)> ApproveExpenseAsync(int id);
        Task<(bool Success, string? Error)> RejectExpenseAsync(int id, string reason);

        Task<(List<StaffMemberItem>? Data, string? Error)> GetStaffsAsync();
        Task<(StaffMemberItem? Data, string? Error)> CreateStaffAsync(CreateStaffFormModel model);
        Task<(bool Success, string? Message, string? Error)> SetStaffActiveAsync(int staffId, bool isActive);
    }
}
