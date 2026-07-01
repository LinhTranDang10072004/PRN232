using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class MonthClosingRepository : IMonthClosingRepository
    {
        private readonly ExpenseDbContext _context;

        public MonthClosingRepository(ExpenseDbContext context) => _context = context;

        public Task<MonthClosing?> GetForMonthAsync(int userId, int month, int year) =>
            MonthClosingDAO.Instance.GetForMonthAsync(_context, userId, month, year);

        public Task<bool> ExistsForMonthAsync(int userId, int month, int year) =>
            MonthClosingDAO.Instance.ExistsForMonthAsync(_context, userId, month, year);

        public Task AddAsync(MonthClosing closing) =>
            MonthClosingDAO.Instance.AddAsync(_context, closing);
    }
}
