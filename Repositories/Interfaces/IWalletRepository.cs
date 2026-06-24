using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IWalletRepository
    {
        IQueryable<Wallet> GetForPersonalUser(int userId);
        Task<Wallet?> GetByIdForUserAsync(int userId, int id);
        Task<bool> IsUsedByExpensesAsync(int walletId);
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
    }
}
