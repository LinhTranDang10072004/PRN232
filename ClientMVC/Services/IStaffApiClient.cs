using ClientMVC.Models.Staff;

namespace ClientMVC.Services
{
    public interface IStaffApiClient
    {
        Task<UserProfileDto?> GetProfileAsync();
        Task<(bool Ok, string? Error)> UpdateProfileAsync(ProfileFormModel model);
        Task<(bool Ok, string? Error)> ChangePasswordAsync(ChangePasswordFormModel model);
        Task<StaffDashboardDto?> GetDashboardAsync();
        Task<List<StaffExpenseDto>> GetExpensesAsync(string? oDataFilter = null);
        Task<StaffExpenseDto?> GetExpenseAsync(int id);
        Task<(bool Ok, StaffExpenseDto? Data, string? Error)> CreateExpenseAsync(StaffExpenseFormModel model);
        Task<(bool Ok, StaffExpenseDto? Data, string? Error)> UpdateExpenseAsync(int id, StaffExpenseFormModel model);
        Task<(bool Ok, string? Error)> DeleteExpenseAsync(int id);
        Task<List<StaffCategoryDto>> GetCategoriesAsync();
        Task<List<StaffAccountDto>> GetAccountsAsync();
        Task<List<NotificationDto>> GetNotificationsAsync();
        Task<int> GetUnreadCountAsync();
        Task MarkNotificationReadAsync(int id);
        Task MarkAllNotificationsReadAsync();
    }
}
