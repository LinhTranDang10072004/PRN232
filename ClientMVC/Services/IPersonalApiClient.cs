using ClientMVC.Models.Personal;

namespace ClientMVC.Services
{
    public interface IPersonalApiClient
    {
        Task<UserProfileDto?> GetProfileAsync();
        Task<(bool Ok, string? Error)> UpdateProfileAsync(ProfileFormModel model);
        Task<(bool Ok, string? Error)> ChangePasswordAsync(ChangePasswordFormModel model);

        Task<List<WalletDto>> GetWalletsAsync();
        Task<WalletDto?> GetWalletAsync(int id);
        Task<(bool Ok, WalletDto? Data, string? Error)> CreateWalletAsync(WalletFormModel model);
        Task<(bool Ok, WalletDto? Data, string? Error)> UpdateWalletAsync(int id, WalletFormModel model);
        Task<(bool Ok, string? Error)> DeactivateWalletAsync(int id);

        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<(bool Ok, CategoryDto? Data, string? Error)> CreateCategoryAsync(CategoryFormModel model);
        Task<(bool Ok, CategoryDto? Data, string? Error)> UpdateCategoryAsync(int id, CategoryFormModel model);
        Task<(bool Ok, string? Error)> DeactivateCategoryAsync(int id);

        Task<List<BudgetDto>> GetBudgetsAsync(int? month = null, int? year = null);
        Task<(bool Ok, BudgetDto? Data, string? Error)> CreateBudgetAsync(BudgetFormModel model);
        Task<(bool Ok, BudgetDto? Data, string? Error)> UpdateBudgetLimitAsync(int id, decimal limitAmount);

        Task<List<ExpenseDto>> GetExpensesAsync(string? oDataFilter = null, string? oDataOrderBy = null);
        Task<List<ExpenseDto>> GetExpensesForMonthAsync(int year, int month);
        Task<ExpenseDto?> GetExpenseAsync(int id);
        Task<(bool Ok, ExpenseDto? Data, string? Error)> CreateExpenseAsync(ExpenseFormModel model);
        Task<(bool Ok, ExpenseDto? Data, string? Error)> UpdateExpenseAsync(int id, ExpenseFormModel model);
        Task<(bool Ok, string? Error)> DeleteExpenseAsync(int id);

        Task<MonthlySummaryDto?> GetMonthlySummaryAsync(int month, int year);
        Task<List<CategoryReportDto>> GetCategoryReportAsync(int month, int year);
        Task<List<BudgetStatusDto>> GetBudgetStatusAsync(int month, int year);
        Task<YearlyReportDto?> GetYearlyReportAsync(int year);

        Task<List<NotificationDto>> GetNotificationsAsync();
        Task<int> GetUnreadCountAsync();
        Task MarkNotificationReadAsync(int id);
        Task MarkAllNotificationsReadAsync();
    }
}
