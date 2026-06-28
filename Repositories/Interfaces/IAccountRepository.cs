using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetForCompany(int companyId);
        IQueryable<Account> GetForCompanyManage(int companyId);
        Task<Account?> GetByIdForCompanyAsync(int companyId, int id);
        Task<bool> AccountNumberExistsAsync(int companyId, string accountNumber, int? excludeId = null);
        Task AddAsync(Account account);
        Task UpdateAsync(Account account);
    }
}
