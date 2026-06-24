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

        public ExpenseRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Expense> GetForPersonalUser(int userId) =>
            ExpenseDAO.Instance.ForPersonalUser(_context, userId);

        public IQueryable<Expense> GetForStaff(int staffId) =>
            ExpenseDAO.Instance.ForStaff(_context, staffId);

        public async Task<Expense?> GetByIdForPersonalUserAsync(int userId, int id)
        {
            var expense = await ExpenseDAO.Instance.GetByIdAsync(_context, id);
            return expense?.UserId == userId ? expense : null;
        }

        public Task<Expense?> GetByIdForStaffAsync(int staffId, int id) =>
            ExpenseDAO.Instance.GetByIdForStaffAsync(_context, staffId, id);

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
    }
}
