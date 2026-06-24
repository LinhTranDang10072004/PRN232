using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class BudgetRepository : IBudgetRepository
    {
        private readonly ExpenseDbContext _context;

        public BudgetRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Budget> GetForPersonalUser(int userId) =>
            BudgetDAO.Instance.ForPersonalUser(_context, userId);

        public async Task<Budget?> GetByIdForUserAsync(int userId, int id)
        {
            var budget = await BudgetDAO.Instance.GetByIdWithDetailsAsync(_context, id);
            return budget?.UserId == userId ? budget : null;
        }

        public Task<Budget?> FindForMonthAsync(int userId, int categoryId, int month, int year) =>
            BudgetDAO.Instance.FindForMonthAsync(_context, userId, categoryId, month, year);

        public Task<bool> ExistsForMonthAsync(int userId, int categoryId, int month, int year, int? excludeId = null) =>
            BudgetDAO.Instance.ExistsForMonthAsync(_context, userId, categoryId, month, year, excludeId);

        public Task AddAsync(Budget budget) =>
            BudgetDAO.Instance.AddAsync(_context, budget);

        public Task UpdateAsync(Budget budget) =>
            BudgetDAO.Instance.UpdateAsync(_context, budget);

        public Task<BudgetDetail?> GetDetailByIdAsync(int detailId) =>
            BudgetDAO.Instance.GetDetailByIdAsync(_context, detailId);

        public Task UpdateDetailAsync(BudgetDetail detail) =>
            BudgetDAO.Instance.UpdateDetailAsync(_context, detail);
    }
}
