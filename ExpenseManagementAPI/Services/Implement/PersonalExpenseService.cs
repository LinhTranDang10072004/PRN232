using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using DataAccessObjects.Context;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalExpenseService : IPersonalExpenseService
    {
        private readonly ExpenseDbContext _context;
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IBudgetRepository _budgetRepository;
        private readonly IPersonalNotificationService _notificationService;

        public PersonalExpenseService(
            ExpenseDbContext context,
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IWalletRepository walletRepository,
            IBudgetRepository budgetRepository,
            IPersonalNotificationService notificationService)
        {
            _context = context;
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

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var expense = new Expense
                {
                    Title = request.Title.Trim(),
                    Description = request.Description?.Trim(),
                    Amount = request.Amount,
                    ExpenseDate = request.ExpenseDate,
                    CategoryId = request.CategoryId,
                    WalletId = request.WalletId,
                    UserId = userId,
                    Status = ExpenseStatusRules.InitialStatusForRole(UserRole.User),
                    CreatedAt = DateTime.UtcNow
                };

                var (walletOk, walletError) = await DeductWalletAsync(userId, request.WalletId, request.Amount);
                if (!walletOk)
                {
                    await tx.RollbackAsync();
                    return (false, null, walletError);
                }

                var (budgetOk, budgetError) = await ApplyBudgetLinkAsync(userId, expense, request);
                if (!budgetOk)
                {
                    await tx.RollbackAsync();
                    return (false, null, budgetError);
                }

                await _expenseRepository.AddAsync(expense);
                await tx.CommitAsync();

                await _notificationService.NotifyAsync(userId, "Tạo khoản chi thành công",
                    $"Đã ghi nhận \"{expense.Title}\" - {expense.Amount:N0}.");

                if (expense.BudgetDetailId.HasValue)
                    await NotifyBudgetAsync(userId, expense.BudgetDetailId.Value);

                var created = await _expenseRepository.GetByIdForPersonalUserAsync(userId, expense.Id);
                return (true, Map(created!), null);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<(bool Success, ExpenseResponse? Data, string? Error)> UpdateAsync(
            int userId, int id, ExpenseRequest request)
        {
            var (valid, error) = await ValidateRequestAsync(userId, request);
            if (!valid)
                return (false, null, error);

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var expense = await _expenseRepository.GetByIdTrackedAsync(id);
                if (expense == null || expense.UserId != userId)
                {
                    await tx.RollbackAsync();
                    return (false, null, "Không tìm thấy khoản chi.");
                }

                // Hoàn tác hiệu ứng cũ (ví + budget)
                await ReverseEffectsAsync(userId, expense);

                expense.Title = request.Title.Trim();
                expense.Description = request.Description?.Trim();
                expense.Amount = request.Amount;
                expense.ExpenseDate = request.ExpenseDate;
                expense.CategoryId = request.CategoryId;
                expense.WalletId = request.WalletId;
                expense.BudgetDetailId = null;

                var (walletOk, walletError) = await DeductWalletAsync(userId, request.WalletId, request.Amount);
                if (!walletOk)
                {
                    await tx.RollbackAsync();
                    return (false, null, walletError);
                }

                var (budgetOk, budgetError) = await ApplyBudgetLinkAsync(userId, expense, request);
                if (!budgetOk)
                {
                    await tx.RollbackAsync();
                    return (false, null, budgetError);
                }

                await _expenseRepository.UpdateAsync(expense);
                await tx.CommitAsync();

                if (expense.BudgetDetailId.HasValue)
                    await NotifyBudgetAsync(userId, expense.BudgetDetailId.Value);

                var updated = await _expenseRepository.GetByIdForPersonalUserAsync(userId, id);
                return (true, Map(updated!), null);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int userId, int id)
        {
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                var expense = await _expenseRepository.GetByIdTrackedAsync(id);
                if (expense == null || expense.UserId != userId)
                {
                    await tx.RollbackAsync();
                    return (false, "Không tìm thấy khoản chi.");
                }

                await ReverseEffectsAsync(userId, expense);
                await _expenseRepository.DeleteAsync(expense);
                await tx.CommitAsync();
                return (true, null);
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<(bool Valid, string? Error)> ValidateRequestAsync(int userId, ExpenseRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Title))
                return (false, "Tiêu đề không được rỗng.");

            if (request.Amount <= 0)
                return (false, "Số tiền chi phải lớn hơn 0.");

            var categories = _categoryRepository.GetForPersonalUser(userId);
            if (!await categories.AnyAsync(c => c.Id == request.CategoryId))
                return (false, "Danh mục không hợp lệ.");

            var wallet = await _walletRepository.GetByIdForUserAsync(userId, request.WalletId);
            if (wallet == null || wallet.Status == "Inactive")
                return (false, "Ví không hợp lệ.");

            if (request.BudgetDetailId.HasValue)
            {
                var detail = await _budgetRepository.GetDetailByIdAsync(request.BudgetDetailId.Value);
                if (detail?.Budget == null || detail.Budget.UserId != userId)
                    return (false, "BudgetDetail không hợp lệ.");

                if (!PersonalExpenseRules.ExpenseDateMatchesBudget(
                        request.ExpenseDate, detail.Budget.Month, detail.Budget.Year))
                    return (false, "Ngày chi không khớp tháng/năm của budget.");
            }

            return (true, null);
        }

        private async Task<(bool Ok, string? Error)> ApplyBudgetLinkAsync(
            int userId, Expense expense, ExpenseRequest request)
        {
            BudgetDetail? detail;

            if (request.BudgetDetailId.HasValue)
            {
                detail = await _budgetRepository.GetDetailByIdAsync(request.BudgetDetailId.Value);
                if (detail?.Budget == null || detail.Budget.UserId != userId)
                    return (false, "BudgetDetail không hợp lệ.");
            }
            else
            {
                if (!expense.CategoryId.HasValue)
                    return (true, null);

                var budget = await _budgetRepository.FindForMonthAsync(
                    userId, expense.CategoryId.Value, request.ExpenseDate.Month, request.ExpenseDate.Year);

                detail = budget?.Details.FirstOrDefault();
                if (detail == null)
                    return (true, null);
            }

            if (!PersonalExpenseRules.ExpenseDateMatchesBudget(
                    request.ExpenseDate, detail.Budget?.Month, detail.Budget?.Year))
                return (false, "Ngày chi không khớp tháng/năm của budget.");

            detail.CurrentAmount += expense.Amount;
            detail.UpdatedAt = DateTime.UtcNow;
            expense.BudgetDetailId = detail.Id;
            await _budgetRepository.UpdateDetailAsync(detail);
            return (true, null);
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

        private async Task<(bool Ok, string? Error)> DeductWalletAsync(int userId, int walletId, decimal amount)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, walletId);
            if (wallet == null)
                return (false, "Ví không hợp lệ.");

            var (ok, error) = PersonalExpenseRules.ValidateWalletBalance(wallet.Balance, amount);
            if (!ok)
                return (false, error);

            wallet.Balance -= amount;
            await _walletRepository.UpdateAsync(wallet);
            return (true, null);
        }

        private async Task RefundWalletAsync(int userId, int walletId, decimal amount)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, walletId);
            if (wallet == null)
                return;

            wallet.Balance += amount;
            await _walletRepository.UpdateAsync(wallet);
        }

        private async Task NotifyBudgetAsync(int userId, int budgetDetailId)
        {
            var detail = await _budgetRepository.GetDetailByIdAsync(budgetDetailId);
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
