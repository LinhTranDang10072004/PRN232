using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalNotificationQueryService
    {
        IQueryable<NotificationResponse> GetForUser(int userId);
        Task<int> GetUnreadCountAsync(int userId);
        Task<(bool Success, string? Error)> MarkAsReadAsync(int userId, int id);
        Task MarkAllAsReadAsync(int userId);
    }
}
