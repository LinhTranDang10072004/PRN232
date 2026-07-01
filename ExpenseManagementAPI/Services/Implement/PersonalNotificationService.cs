using BusinessObjects.Models;
using BusinessObjects.Validation;
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

        public Task NotifyAsync(int userId, string title, string? content = null, string severity = "info") =>
            _notificationRepository.AddAsync(new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                Severity = severity,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            });

        public async Task CheckBudgetAlertsAsync(
            int userId, int budgetId, string budgetName, decimal limit, decimal carryOverDebt, decimal spent)
        {
            var effectiveLimit = PersonalBudgetRules.EffectiveLimit(limit, carryOverDebt);
            var remaining = effectiveLimit - spent;

            if (spent > effectiveLimit)
            {
                var overflow = PersonalBudgetRules.OverflowAmount(limit, spent);
                await NotifyAsync(userId,
                    "🔴 Vượt ngân sách",
                    overflow > 0
                        ? $"Bạn đã vượt ngân sách \"{budgetName}\". Đã chi {spent:N0} / hạn mức {limit:N0} (vượt {overflow:N0})."
                        : $"Bạn đã vượt hạn mức khả dụng \"{budgetName}\". Đã chi {spent:N0} / khả dụng {effectiveLimit:N0}.",
                    "danger");
                return;
            }

            if (effectiveLimit > 0 && remaining <= effectiveLimit * 0.2m)
            {
                await NotifyAsync(userId,
                    $"Budget {budgetName} sắp hết",
                    $"Còn lại {remaining:N0} trong hạn mức khả dụng {effectiveLimit:N0}.",
                    "warning");
            }
        }
    }
}
