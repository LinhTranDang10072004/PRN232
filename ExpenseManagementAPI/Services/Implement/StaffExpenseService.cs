using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using ExpenseManagementAPI.DTOs.Staff;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class StaffExpenseService : IStaffExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPersonalNotificationService _notificationService;

        public StaffExpenseService(
            IExpenseRepository expenseRepository,
            ICategoryRepository categoryRepository,
            IAccountRepository accountRepository,
            IPersonalNotificationService notificationService)
        {
            _expenseRepository = expenseRepository;
            _categoryRepository = categoryRepository;
            _accountRepository = accountRepository;
            _notificationService = notificationService;
        }

        public IQueryable<StaffExpenseResponse> GetForStaff(int staffId) =>
            _expenseRepository.GetForStaff(staffId).Select(e => new StaffExpenseResponse
            {
                Id = e.Id,
                Title = e.Title,
                Description = e.Description,
                Amount = e.Amount,
                ExpenseDate = e.ExpenseDate,
                Status = e.Status,
                CategoryId = e.CategoryId,
                CategoryName = e.Category != null ? e.Category.Name : null,
                AccountId = e.AccountId,
                AccountName = e.Account != null ? e.Account.Name : null,
                AccountNumber = e.Account != null ? e.Account.AccountNumber : null,
                CreatedAt = e.CreatedAt
            });

        public async Task<(bool Success, StaffExpenseResponse? Data, string? Error)> GetByIdAsync(int staffId, int id)
        {
            var expense = await _expenseRepository.GetByIdForStaffAsync(staffId, id);
            return expense == null
                ? (false, null, "Không tìm thấy phiếu chi.")
                : (true, Map(expense), null);
        }

        public async Task<(bool Success, StaffExpenseResponse? Data, string? Error)> CreateAsync(
            int staffId, int companyId, StaffExpenseRequest request)
        {
            var (valid, error) = await ValidateRequestAsync(companyId, request);
            if (!valid)
                return (false, null, error);

            var expense = new Expense
            {
                Title = request.Title.Trim(),
                Description = request.Description?.Trim(),
                Amount = request.Amount,
                ExpenseDate = request.ExpenseDate ?? DateTime.UtcNow,
                CategoryId = request.CategoryId,
                AccountId = request.AccountId,
                UserId = staffId,
                Status = ExpenseStatusRules.InitialStatusForRole(UserRole.CompanyStaff),
                CreatedAt = DateTime.UtcNow
            };

            await _expenseRepository.AddAsync(expense);
            await _notificationService.NotifyAsync(staffId, "Gửi phiếu chi thành công",
                $"Phiếu \"{expense.Title}\" - {expense.Amount:N0} đ đang chờ duyệt.");

            var created = await _expenseRepository.GetByIdForStaffAsync(staffId, expense.Id);
            return (true, Map(created!), null);
        }

        public async Task<(bool Success, StaffExpenseResponse? Data, string? Error)> UpdateAsync(
            int staffId, int companyId, int id, StaffExpenseRequest request)
        {
            var expense = await _expenseRepository.GetByIdTrackedAsync(id);
            if (expense == null || expense.UserId != staffId)
                return (false, null, "Không tìm thấy phiếu chi.");

            if (!ExpenseStatusRules.CanUpdate(UserRole.CompanyStaff, expense.Status))
                return (false, null, "Phiếu đã được duyệt, không thể sửa.");

            var (valid, error) = await ValidateRequestAsync(companyId, request);
            if (!valid)
                return (false, null, error);

            expense.Title = request.Title.Trim();
            expense.Description = request.Description?.Trim();
            expense.Amount = request.Amount;
            expense.ExpenseDate = request.ExpenseDate ?? expense.ExpenseDate;
            expense.CategoryId = request.CategoryId;
            expense.AccountId = request.AccountId;

            if (expense.Status == ExpenseStatus.Rejected)
                expense.Status = ExpenseStatus.Pending;

            await _expenseRepository.UpdateAsync(expense);

            var updated = await _expenseRepository.GetByIdForStaffAsync(staffId, id);
            return (true, Map(updated!), null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(int staffId, int id)
        {
            var expense = await _expenseRepository.GetByIdTrackedAsync(id);
            if (expense == null || expense.UserId != staffId)
                return (false, "Không tìm thấy phiếu chi.");

            if (!ExpenseStatusRules.CanDelete(UserRole.CompanyStaff, expense.Status))
                return (false, "Phiếu đã được duyệt, không thể xóa.");

            await _expenseRepository.DeleteAsync(expense);
            return (true, null);
        }

        public async Task<StaffDashboardResponse> GetDashboardAsync(int staffId)
        {
            var expenses = await _expenseRepository.GetForStaff(staffId).ToListAsync();
            return new StaffDashboardResponse
            {
                PendingCount = expenses.Count(e => e.Status == ExpenseStatus.Pending),
                ApprovedCount = expenses.Count(e => e.Status == ExpenseStatus.Approved),
                RejectedCount = expenses.Count(e => e.Status == ExpenseStatus.Rejected),
                TotalPendingAmount = expenses
                    .Where(e => e.Status == ExpenseStatus.Pending)
                    .Sum(e => e.Amount)
            };
        }

        private async Task<(bool Valid, string? Error)> ValidateRequestAsync(int companyId, StaffExpenseRequest request)
        {
            var categories = _categoryRepository.GetForCorporateCompany(companyId);
            if (!await categories.AnyAsync(c => c.Id == request.CategoryId))
                return (false, "Danh mục không hợp lệ.");

            var account = await _accountRepository.GetByIdForCompanyAsync(companyId, request.AccountId);
            if (account == null || !account.IsActive)
                return (false, "Tài khoản công ty không hợp lệ.");

            return (true, null);
        }

        private static StaffExpenseResponse Map(Expense e) => new()
        {
            Id = e.Id,
            Title = e.Title,
            Description = e.Description,
            Amount = e.Amount,
            ExpenseDate = e.ExpenseDate,
            Status = e.Status,
            CategoryId = e.CategoryId,
            CategoryName = e.Category?.Name,
            AccountId = e.AccountId,
            AccountName = e.Account?.Name,
            AccountNumber = e.Account?.AccountNumber,
            CreatedAt = e.CreatedAt,
            ApprovalHistories = e.ApprovalHistories
                .OrderByDescending(h => h.ActionDate)
                .Select(h => new ApprovalHistoryResponse
                {
                    Id = h.Id,
                    Action = h.Action,
                    Comment = h.Comment,
                    AdminName = h.Admin?.FullName ?? h.Admin?.UserName,
                    ActionDate = h.ActionDate
                })
                .ToList()
        };
    }
}
