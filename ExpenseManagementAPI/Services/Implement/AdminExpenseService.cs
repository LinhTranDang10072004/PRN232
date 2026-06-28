using BusinessObjects.Enums;
using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.DTOs.Staff;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class AdminExpenseService : IAdminExpenseService
    {
        private readonly IExpenseRepository _expenseRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPersonalNotificationService _notificationService;

        public AdminExpenseService(
            IExpenseRepository expenseRepository,
            IUserRepository userRepository,
            IPersonalNotificationService notificationService)
        {
            _expenseRepository = expenseRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
        }

        public IQueryable<AdminExpenseResponse> GetForCompany(int companyId) =>
            _expenseRepository.GetForAdminCompany(companyId).Select(e => new AdminExpenseResponse
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
                CreatedAt = e.CreatedAt,
                StaffUserId = e.UserId,
                StaffUserName = e.User != null ? e.User.UserName : null,
                StaffFullName = e.User != null ? e.User.FullName : null
            });

        public async Task<(bool Success, AdminExpenseResponse? Data, string? Error)> GetByIdAsync(int companyId, int id)
        {
            var expense = await _expenseRepository.GetByIdForAdminAsync(companyId, id);
            return expense == null
                ? (false, null, "Không tìm thấy phiếu chi.")
                : (true, Map(expense), null);
        }

        public async Task<(bool Success, string? Error)> ApproveAsync(int adminId, int companyId, int expenseId)
        {
            var (success, error, staffUserId) = await _expenseRepository.ApproveForAdminAsync(adminId, companyId, expenseId);
            if (!success)
                return (false, error);

            if (staffUserId.HasValue)
            {
                await _notificationService.NotifyAsync(staffUserId.Value,
                    "Phiếu chi đã được duyệt",
                    $"Phiếu chi #{expenseId} đã được Admin phê duyệt.");
            }

            return (true, null);
        }

        public async Task<(bool Success, string? Error)> RejectAsync(
            int adminId, int companyId, int expenseId, string comment)
        {
            var (success, error, staffUserId) = await _expenseRepository.RejectForAdminAsync(
                adminId, companyId, expenseId, comment);
            if (!success)
                return (false, error);

            if (staffUserId.HasValue)
            {
                await _notificationService.NotifyAsync(staffUserId.Value,
                    "Phiếu chi bị từ chối",
                    $"Phiếu chi #{expenseId} bị từ chối. Lý do: {comment.Trim()}");
            }

            return (true, null);
        }

        public async Task<AdminDashboardResponse> GetDashboardAsync(int companyId)
        {
            var expenses = await _expenseRepository.GetForAdminCompany(companyId).ToListAsync();
            var staff = await _userRepository.GetStaffByCompanyAsync(companyId);

            return new AdminDashboardResponse
            {
                PendingCount = expenses.Count(e => e.Status == ExpenseStatus.Pending),
                ApprovedCount = expenses.Count(e => e.Status == ExpenseStatus.Approved),
                RejectedCount = expenses.Count(e => e.Status == ExpenseStatus.Rejected),
                TotalPendingAmount = expenses
                    .Where(e => e.Status == ExpenseStatus.Pending)
                    .Sum(e => e.Amount),
                StaffCount = staff.Count
            };
        }

        private static AdminExpenseResponse Map(Expense e) => new()
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
            StaffUserId = e.UserId,
            StaffUserName = e.User?.UserName,
            StaffFullName = e.User?.FullName,
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
