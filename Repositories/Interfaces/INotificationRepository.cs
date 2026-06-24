using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface INotificationRepository
    {
        IQueryable<Notification> GetForUser(int userId);
        Task<Notification?> GetByIdForUserAsync(int userId, int id);
        Task<int> GetUnreadCountAsync(int userId);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(Notification notification);
        Task MarkAllAsReadAsync(int userId);
    }
}
