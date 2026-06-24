using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalExpenseService : IPersonalExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IPersonalNotificationService _notificationService;

        public PersonalExpenseService(
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IWalletRepository walletRepository,
            IBudgetRepository budgetRepository,
            IPersonalNotificationService notificationService)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _walletRepository = walletRepository;
            _budgetRepository = budgetRepository;
            _notificationService = notificationService;
        }

        public IQueryable<ExpenseResponse> GetForUser(int userId) =>
            _expenseRepository.GetForPersonalUser(userId).Select(e => new ExpenseResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Status = e.Status,
                CategoryId = e.CategoryId,
                CategoryName = e.Category != null ? e.Category.Name : null,
                WalletId = e.WalletId,
                WalletName = e.Wallet != null ? e.Wallet.Name : null,
                BudgetDetailId = e.BudgetDetailId,
                CreatedAt = e.CreatedAt
            });

        public async Task<(bool Success, ExpenseResponse? Data, string? Error)> GetByIdAsync(int userId, int id)
        {
            var expense = await _expenseRepository.GetByIdForPersonalUserAsync(userId, id);
            return expense == null
                ? (false, null, "Không tìm thấy khoản chi.")
                : (true, Map(expense), null);
        }

        public async Task<(bool Success, ExpenseResponse? Data, string? Error)> CreateAsync(
            int userId, ExpenseRequest request)
        {
            var (valid, error) = await ValidateRequestAsync(userId, request);
            if (!valid)
                return (false, null, error);

            var expenseDate = request.ExpenseDate ?? DateTime.UtcNow;
            var expense = new Expense
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Amount = request.Amount,
                ExpenseDate = expenseDate,
                CategoryId = request.CategoryId,
                WalletId = request.WalletId,
                UserId = userId,
                Status = ExpenseStatusRules.InitialStatusForRole(UserRole.User),
                CreatedAt = DateTime.UtcNow
            };

            await ApplyBudgetLinkAsync(userId, expense, expenseDate);
            if (request.WalletId.HasValue)
                await DeductWalletAsync(userId, request.WalletId.Value, request.Amount);

            await _expenseRepository.AddAsync(expense);
            await _notificationService.NotifyAsync(userId, "Tạo khoản chi thành công",
                $"Đã ghi nhận \"{expense.Title}\" - {expense.Amount:N0}.");

            if (expense.BudgetDetailId.HasValue)
                await NotifyBudgetAsync(userId, expense);

            var created = await _expenseRepository.GetByIdForPersonalUserAsync(userId, expense.Id);
            return (true, Map(created!), null);
        }

        public async Task<(bool Success, ExpenseResponse? Data, string? Error)> UpdateAsync(
            int userId, int id, ExpenseRequest request)
        {
            var expense = await _expenseRepository.GetByIdTrackedAsync(id);
            if (expense == null || expense.UserId != userId)
                return (false, null, "Không tìm thấy khoản chi.");

            var (valid, error) = await ValidateRequestAsync(userId, request);
            if (!valid)
                return (false, null, error);

            await ReverseEffectsAsync(userId, expense);

            expense.Title = request.Title.Trim();
            expense.Description = request.Description?.Trim();
            expense.Amount = request.Amount;
            expense.ExpenseDate = request.ExpenseDate ?? expense.ExpenseDate;
            expense.CategoryId = request.CategoryId;
            expense.WalletId = request.WalletId;
            expense.BudgetDetailId = null;

            var expenseDate = expense.ExpenseDate ?? DateTime.UtcNow;
            await ApplyBudgetLinkAsync(userId, expense, expenseDate);
            if (request.WalletId.HasValue)
                await DeductWalletAsync(userId, request.WalletId.Value, request.Amount);

            await _expenseRepository.UpdateAsync(expense);
            if (expense.BudgetDetailId.HasValue)
                await NotifyBudgetAsync(userId, expense);

            var updated = await _expenseRepository.GetByIdForPersonalUserAsync(userId, id);
            return (true, Map(updated!), null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int userId, int id)
        {
            var expense = await _expenseRepository.GetByIdTrackedAsync(id);
            if (expense == null || expense.UserId != userId)
                return (false, "Không tìm thấy khoản chi.");

            await ReverseEffectsAsync(userId, expense);
            await _expenseRepository.DeleteAsync(expense);
            return (true, null);
        }

        private async Task<(bool Valid, string? Error)> ValidateRequestAsync(int userId, ExpenseRequest request)
        {
            var categories = _categoryRepository.GetForPersonalUser(userId);
            if (!await categories.AnyAsync(c => c.Id == request.CategoryId))
                return (false, "Danh mục không hợp lệ.");

            if (request.WalletId.HasValue)
            {
                var wallet = await _walletRepository.GetByIdForUserAsync(userId, request.WalletId.Value);
                if (wallet == null || wallet.Status == "Inactive")
                    return (false, "Ví không hợp lệ.");
            }

            return (true, null);
        }

        private async Task ApplyBudgetLinkAsync(int userId, Expense expense, DateTime expenseDate)
        {
            if (!expense.CategoryId.HasValue)
                return;

            var budget = await _budgetRepository.FindForMonthAsync(
                userId, expense.CategoryId.Value, expenseDate.Month, expenseDate.Year);

            var detail = budget?.Details.FirstOrDefault();
            if (detail == null)
                return;

            detail.CurrentAmount += expense.Amount;
            detail.UpdatedAt = DateTime.UtcNow;
            expense.BudgetDetailId = detail.Id;
            await _budgetRepository.UpdateDetailAsync(detail);
        }

        private async Task ReverseEffectsAsync(int userId, Expense expense)
        {
            if (expense.BudgetDetailId.HasValue)
            {
                var detail = await _budgetRepository.GetDetailByIdAsync(expense.BudgetDetailId.Value);
                if (detail != null)
                {
                    detail.CurrentAmount -= expense.Amount;
                    if (detail.CurrentAmount < 0)
                        detail.CurrentAmount = 0;
                    detail.UpdatedAt = DateTime.UtcNow;
                    await _budgetRepository.UpdateDetailAsync(detail);
                }
            }

            if (expense.WalletId.HasValue)
                await RefundWalletAsync(userId, expense.WalletId.Value, expense.Amount);
        }

        private async Task DeductWalletAsync(int userId, int walletId, decimal amount)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, walletId);
            if (wallet == null)
                throw new InvalidOperationException("Ví không hợp lệ.");

            wallet.Balance -= amount;
            await _walletRepository.UpdateAsync(wallet);
        }

        private async Task RefundWalletAsync(int userId, int walletId, decimal amount)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, walletId);
            if (wallet == null)
                return;

            wallet.Balance += amount;
            await _walletRepository.UpdateAsync(wallet);
        }

        private async Task NotifyBudgetAsync(int userId, Expense expense)
        {
            if (!expense.BudgetDetailId.HasValue)
                return;

            var detail = await _budgetRepository.GetDetailByIdAsync(expense.BudgetDetailId.Value);
            if (detail?.Budget == null)
                return;

            await _notificationService.CheckBudgetAlertsAsync(
                userId,
                detail.Budget.Id,
                detail.Budget.Name,
                detail.LimitAmount,
                detail.CurrentAmount);
        }

        private static ExpenseResponse Map(Expense e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Amount = e.Amount,
            ExpenseDate = e.ExpenseDate,
            Status = e.Status,
            CategoryId = e.CategoryId,
            CategoryName = e.Category?.Name,
            WalletId = e.WalletId,
            WalletName = e.Wallet?.Name,
            BudgetDetailId = e.BudgetDetailId,
            CreatedAt = e.CreatedAt
        };
    }
}
