using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ExpenseDbContext _context;

        public NotificationRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Notification> GetForUser(int userId) =>
            NotificationDAO.Instance.ForUser(_context, userId);

        public async Task<Notification?> GetByIdForUserAsync(int userId, int id)
        {
            var notification = await NotificationDAO.Instance.GetByIdAsync(_context, id);
            return notification?.UserId == userId ? notification : null;
        }

        public Task<int> GetUnreadCountAsync(int userId) =>
            NotificationDAO.Instance.GetUnreadCountAsync(_context, userId);

        public Task AddAsync(Notification notification) =>
            NotificationDAO.Instance.AddAsync(_context, notification);

        public Task MarkAsReadAsync(Notification notification) =>
            NotificationDAO.Instance.MarkAsReadAsync(_context, notification);

        public Task MarkAllAsReadAsync(int userId) =>
            NotificationDAO.Instance.MarkAllAsReadAsync(_context, userId);
    }
}
