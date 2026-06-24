using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class NotificationDAO
    {
        private static NotificationDAO? _instance;
        private static readonly object _lock = new();

        private NotificationDAO() { }

        public static NotificationDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                        _instance ??= new NotificationDAO();
                }
                return _instance;
            }
        }

        public IQueryable<Notification> ForUser(ExpenseDbContext context, int userId) =>
            context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .AsNoTracking();

        public async Task<Notification?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Notifications.FindAsync(id);

        public async Task<int> GetUnreadCountAsync(ExpenseDbContext context, int userId) =>
            await context.Notifications.CountAsync(n => n.UserId == userId && !n.IsRead);

        public async Task AddAsync(ExpenseDbContext context, Notification notification)
        {
            await context.Notifications.AddAsync(notification);
            await context.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(ExpenseDbContext context, Notification notification)
        {
            notification.IsRead = true;
            context.Notifications.Update(notification);
            await context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(ExpenseDbContext context, int userId)
        {
            await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ExecuteUpdateAsync(s => s.SetProperty(n => n.IsRead, true));
        }
    }
}
