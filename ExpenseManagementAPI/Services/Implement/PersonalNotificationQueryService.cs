using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalNotificationQueryService : IPersonalNotificationQueryService
    {
        private readonly INotificationRepository _notificationRepository;

        public PersonalNotificationQueryService(INotificationRepository notificationRepository) =>
            _notificationRepository = notificationRepository;

        public IQueryable<NotificationResponse> GetForUser(int userId) =>
            _notificationRepository.GetForUser(userId).Select(n => new NotificationResponse
            {
                Id = n.Id,
                Title = n.Title,
                Content = n.Content,
                IsRead = n.IsRead,
                Severity = n.Severity,
                CreatedAt = n.CreatedAt
            });

        public Task<int> GetUnreadCountAsync(int userId) =>
            _notificationRepository.GetUnreadCountAsync(userId);

        public async Task<(bool Success, string? Error)> MarkAsReadAsync(int userId, int id)
        {
            var notification = await _notificationRepository.GetByIdForUserAsync(userId, id);
            if (notification == null)
                return (false, "Không tìm thấy thông báo.");

            await _notificationRepository.MarkAsReadAsync(notification);
            return (true, null);
        }

        public Task MarkAllAsReadAsync(int userId) =>
            _notificationRepository.MarkAllAsReadAsync(userId);
    }
}
