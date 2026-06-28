using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _context;

        public ExpenseRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Expense> GetForPersonalUser(int userId) =>
            ExpenseDAO.Instance.ForPersonalUser(_context, userId);

        public IQueryable<Expense> GetForStaff(int staffId) =>
            ExpenseDAO.Instance.ForStaff(_context, staffId);

        public IQueryable<Expense> GetForAdminCompany(int companyId) =>
            ExpenseDAO.Instance.ForAdminCompany(_context, companyId);

        public async Task<Expense?> GetByIdForPersonalUserAsync(int userId, int id)
        {
            var expense = await ExpenseDAO.Instance.GetByIdAsync(_context, id);
            return expense?.UserId == userId ? expense : null;
        }

        public Task<Expense?> GetByIdForStaffAsync(int staffId, int id) =>
            ExpenseDAO.Instance.GetByIdForStaffAsync(_context, staffId, id);

        public Task<Expense?> GetByIdForAdminAsync(int companyId, int expenseId) =>
            ExpenseDAO.Instance.GetByIdForAdminAsync(_context, companyId, expenseId);

        public async Task<Expense?> GetByIdTrackedAsync(int id) =>
            await _context.Expenses
                .Include(e => e.Category)
                .Include(e => e.Wallet)
                .Include(e => e.Account)
                .FirstOrDefaultAsync(e => e.Id == id);

        public Task AddAsync(Expense expense) =>
            ExpenseDAO.Instance.AddAsync(_context, expense);

        public Task UpdateAsync(Expense expense) =>
            ExpenseDAO.Instance.UpdateAsync(_context, expense);

        public Task DeleteAsync(Expense expense) =>
            ExpenseDAO.Instance.DeleteAsync(_context, expense);

        public async Task<(bool Success, string? Error, int? StaffUserId)> ApproveForAdminAsync(
            int adminId, int companyId, int expenseId)
        {
            var expense = await LoadCorporateExpenseTrackedAsync(companyId, expenseId);
            if (expense == null)
                return (false, "Không tìm thấy phiếu chi.", null);
            if (expense.Status != ExpenseStatus.Pending)
                return (false, "Chỉ duyệt được phiếu đang chờ.", null);

            expense.Status = ExpenseStatus.Approved;
            await _context.ApprovalHistories.AddAsync(new ApprovalHistory
            {
                ExpenseId = expense.Id,
                AdminId = adminId,
                Action = ApprovalHistoryRules.ActionApproved,
                ActionDate = DateTime.UtcNow,
                AccountId = expense.AccountId
            });
            await _context.SaveChangesAsync();
            return (true, null, expense.UserId);
        }

        public async Task<(bool Success, string? Error, int? StaffUserId)> RejectForAdminAsync(
            int adminId, int companyId, int expenseId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                return (false, "Vui lòng nhập lý do từ chối.", null);

            var expense = await LoadCorporateExpenseTrackedAsync(companyId, expenseId);
            if (expense == null)
                return (false, "Không tìm thấy phiếu chi.", null);
            if (expense.Status != ExpenseStatus.Pending)
                return (false, "Chỉ từ chối được phiếu đang chờ.", null);

            expense.Status = ExpenseStatus.Rejected;
            await _context.ApprovalHistories.AddAsync(new ApprovalHistory
            {
                ExpenseId = expense.Id,
                AdminId = adminId,
                Action = ApprovalHistoryRules.ActionRejected,
                Comment = comment.Trim(),
                ActionDate = DateTime.UtcNow,
                AccountId = expense.AccountId
            });
            await _context.SaveChangesAsync();
            return (true, null, expense.UserId);
        }

        private async Task<Expense?> LoadCorporateExpenseTrackedAsync(int companyId, int expenseId) =>
            await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e =>
                    e.Id == expenseId &&
                    e.User != null &&
                    e.User.Role == UserRole.CompanyStaff &&
                    e.User.CompanyId == companyId);
    }
}
