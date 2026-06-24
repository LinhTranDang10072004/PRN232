namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalNotificationService
    {
        Task NotifyAsync(int userId, string title, string? content = null);
        Task CheckBudgetAlertsAsync(int userId, int budgetId, string budgetName, decimal limit, decimal spent);
    }
}
