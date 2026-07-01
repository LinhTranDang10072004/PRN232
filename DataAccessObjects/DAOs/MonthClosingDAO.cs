using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class MonthClosingDAO
    {
        private static MonthClosingDAO? _instance;
        private static readonly object _lock = new();

        private MonthClosingDAO() { }

        public static MonthClosingDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                        _instance ??= new MonthClosingDAO();
                }
                return _instance;
            }
        }

        public async Task<MonthClosing?> GetForMonthAsync(ExpenseDbContext context, int userId, int month, int year) =>
            await context.MonthClosings
                .AsNoTracking()
                .FirstOrDefaultAsync(m =>
                    m.UserId == userId &&
                    m.Month == month &&
                    m.Year == year);

        public async Task<bool> ExistsForMonthAsync(ExpenseDbContext context, int userId, int month, int year) =>
            await context.MonthClosings.AnyAsync(m =>
                m.UserId == userId &&
                m.Month == month &&
                m.Year == year);

        public async Task AddAsync(ExpenseDbContext context, MonthClosing closing)
        {
            await context.MonthClosings.AddAsync(closing);
            await context.SaveChangesAsync();
        }
    }
}
