using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class WalletDAO
    {
        private static WalletDAO? _instance;
        private static readonly object _lock = new();

        private WalletDAO() { }

        public static WalletDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                        _instance ??= new WalletDAO();
                }
                return _instance;
            }
        }

        public IQueryable<Wallet> ForPersonalUser(ExpenseDbContext context, int userId) =>
            context.Wallets
                .Where(w => w.UserId == userId)
                .AsNoTracking();

        public async Task<Wallet?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Wallets.FirstOrDefaultAsync(w => w.Id == id);

        public async Task<bool> IsUsedByExpensesAsync(ExpenseDbContext context, int walletId) =>
            await context.Expenses.AnyAsync(e => e.WalletId == walletId);

        public async Task AddAsync(ExpenseDbContext context, Wallet wallet)
        {
            await context.Wallets.AddAsync(wallet);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, Wallet wallet)
        {
            context.Wallets.Update(wallet);
            await context.SaveChangesAsync();
        }
    }
}
