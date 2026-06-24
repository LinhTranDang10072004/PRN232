using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ExpenseDbContext _context;

        public WalletRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Wallet> GetForPersonalUser(int userId) =>
            WalletDAO.Instance.ForPersonalUser(_context, userId);

        public async Task<Wallet?> GetByIdForUserAsync(int userId, int id)
        {
            var wallet = await WalletDAO.Instance.GetByIdAsync(_context, id);
            return wallet?.UserId == userId ? wallet : null;
        }

        public Task<bool> IsUsedByExpensesAsync(int walletId) =>
            WalletDAO.Instance.IsUsedByExpensesAsync(_context, walletId);

        public Task AddAsync(Wallet wallet) =>
            WalletDAO.Instance.AddAsync(_context, wallet);

        public Task UpdateAsync(Wallet wallet) =>
            WalletDAO.Instance.UpdateAsync(_context, wallet);
    }
}
