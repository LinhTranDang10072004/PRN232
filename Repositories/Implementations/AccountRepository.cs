using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class AccountRepository : IAccountRepository
    {
        private readonly ExpenseDbContext _context;

        public AccountRepository(ExpenseDbContext context) => _context = context;

        public IQueryable<Account> GetForCompany(int companyId) =>
            AccountDAO.Instance.ForCompany(_context, companyId);

        public IQueryable<Account> GetForCompanyManage(int companyId) =>
            AccountDAO.Instance.ForCompanyManage(_context, companyId);

        public Task<Account?> GetByIdForCompanyAsync(int companyId, int id) =>
            AccountDAO.Instance.GetByIdForCompanyAsync(_context, companyId, id);

        public Task<bool> AccountNumberExistsAsync(int companyId, string accountNumber, int? excludeId = null) =>
            AccountDAO.Instance.AccountNumberExistsAsync(_context, companyId, accountNumber, excludeId);

        public Task AddAsync(Account account) =>
            AccountDAO.Instance.AddAsync(_context, account);

        public Task UpdateAsync(Account account) =>
            AccountDAO.Instance.UpdateAsync(_context, account);
    }
}
