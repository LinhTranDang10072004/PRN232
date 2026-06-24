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

        public Task<Account?> GetByIdForCompanyAsync(int companyId, int id) =>
            AccountDAO.Instance.GetByIdForCompanyAsync(_context, companyId, id);
    }
}
