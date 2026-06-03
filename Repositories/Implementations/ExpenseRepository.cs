using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Microsoft.EntityFrameworkCore;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly ExpenseDbContext _context;
        private readonly ExpenseDAO _dao = ExpenseDAO.Instance;

        public ExpenseRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Expense> GetForUser(int userId) =>
            _dao.ForPersonalUser(_context, userId);

        public IQueryable<Expense> GetForStaff(int staffId) =>
            _dao.ForStaff(_context, staffId);

        public IQueryable<Expense> GetForAdmin(int adminId) =>
            _dao.ForAdminWorkspace(_context, adminId);

        public Task<Expense?> GetByIdAsync(int id) =>
            _dao.GetByIdAsync(_context, id);

        public Task AddAsync(Expense expense) =>
            _dao.AddAsync(_context, expense);

        public Task UpdateAsync(Expense expense) =>
            _dao.UpdateAsync(_context, expense);

        public Task DeleteAsync(Expense expense) =>
            _dao.DeleteAsync(_context, expense);

        public async Task<bool> ApproveAsync(int expenseId, int adminId)
        {
            var expense = await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (expense == null ||
                expense.User.Role != UserRole.Staff ||
                expense.User.ParentAdminId != adminId ||
                expense.Status != ExpenseStatus.Pending)
            {
                return false;
            }

            expense.Status = ExpenseStatus.Approved;
            expense.ReviewedAt = DateTime.UtcNow;
            expense.ReviewedByAdminId = adminId;
            expense.RejectionReason = null;
            await _dao.UpdateAsync(_context, expense);
            return true;
        }

        public async Task<bool> RejectAsync(int expenseId, int adminId, string reason)
        {
            var expense = await _context.Expenses
                .Include(e => e.User)
                .FirstOrDefaultAsync(e => e.Id == expenseId);

            if (expense == null ||
                expense.User.Role != UserRole.Staff ||
                expense.User.ParentAdminId != adminId ||
                expense.Status != ExpenseStatus.Pending)
            {
                return false;
            }

            expense.Status = ExpenseStatus.Rejected;
            expense.ReviewedAt = DateTime.UtcNow;
            expense.ReviewedByAdminId = adminId;
            expense.RejectionReason = reason;
            await _dao.UpdateAsync(_context, expense);
            return true;
        }
    }
}
