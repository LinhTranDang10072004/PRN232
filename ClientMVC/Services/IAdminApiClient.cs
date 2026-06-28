using ClientMVC.Models.Admin;

namespace ClientMVC.Services
{
    public interface IAdminApiClient
    {
        Task<AdminDashboardDto?> GetDashboardAsync();
        Task<List<AdminExpenseDto>> GetExpensesAsync(string? oDataFilter = null);
        Task<AdminExpenseDto?> GetExpenseAsync(int id);
        Task<(bool Ok, string? Error)> ApproveExpenseAsync(int id);
        Task<(bool Ok, string? Error)> RejectExpenseAsync(int id, string comment);

        Task<List<StaffUserDto>> GetStaffAsync();
        Task<(bool Ok, string? Error)> CreateStaffAsync(CreateStaffFormModel model);
        Task<(bool Ok, string? Error)> UpdateStaffStatusAsync(int id, bool isActive);

        Task<List<AdminCategoryDto>> GetCategoriesAsync();
        Task<(bool Ok, string? Error)> CreateCategoryAsync(AdminCategoryFormModel model);
        Task<(bool Ok, string? Error)> UpdateCategoryAsync(int id, AdminCategoryFormModel model);
        Task<(bool Ok, string? Error)> DeleteCategoryAsync(int id);

        Task<List<AdminAccountDto>> GetAccountsAsync();
        Task<(bool Ok, string? Error)> CreateAccountAsync(AdminAccountFormModel model);
        Task<(bool Ok, string? Error)> UpdateAccountAsync(int id, AdminAccountFormModel model);
    }
}
