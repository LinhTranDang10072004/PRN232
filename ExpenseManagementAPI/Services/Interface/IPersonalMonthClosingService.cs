using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalMonthClosingService
    {
        Task<MonthClosingPreviewResponse> GetClosingPreviewAsync(int userId, int month, int year);
        Task<(bool Success, MonthClosingResponse? Data, string? Error)> CloseMonthAsync(
            int userId, int month, int year, string? notes = null);
        Task<MonthClosingStatusResponse> GetClosingStatusAsync(int userId, int month, int year);
        Task<bool> ShouldWarnPreviousMonthNotClosedAsync(int userId, int month, int year);
    }
}
