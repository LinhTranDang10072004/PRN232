using BusinessObjects.Models;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalNotificationService : IPersonalNotificationService
    {
        private readonly INotificationRepository _notificationRepository;

        public PersonalNotificationService(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        public Task NotifyAsync(int userId, string title, string? content = null) =>
            _notificationRepository.AddAsync(new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

        public async Task CheckBudgetAlertsAsync(
            int userId, int budgetId, string budgetName, decimal limit, decimal spent)
        {
            var remaining = limit - spent;

            if (spent > limit)
            {
                await NotifyAsync(userId,
                    "Vượt ngân sách",
                    $"Bạn đã vượt ngân sách {budgetName}. Đã chi {spent:N0} / hạn mức {limit:N0}.");
                return;
            }

            if (limit > 0 && remaining <= limit * 0.2m)
            {
                await NotifyAsync(userId,
                    $"Budget {budgetName} sắp hết",
                    $"Còn lại {remaining:N0} trong hạn mức {limit:N0}.");
            }
        }
    }
}
