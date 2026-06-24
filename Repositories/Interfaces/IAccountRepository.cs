using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IAccountRepository
    {
        IQueryable<Account> GetForCompany(int companyId);
        Task<Account?> GetByIdForCompanyAsync(int companyId, int id);
    }
}
