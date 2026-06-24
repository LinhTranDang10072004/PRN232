using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalReportService
    {
        Task<MonthlySummaryResponse> GetMonthlySummaryAsync(int userId, int month, int year);
        Task<List<CategoryReportItem>> GetByCategoryAsync(int userId, int month, int year);
        Task<List<WalletReportItem>> GetByWalletAsync(int userId, int month, int year);
        Task<List<BudgetStatusItem>> GetBudgetStatusAsync(int userId, int month, int year);
        Task<YearlyReportResponse> GetYearlyReportAsync(int userId, int year);
    }
}
