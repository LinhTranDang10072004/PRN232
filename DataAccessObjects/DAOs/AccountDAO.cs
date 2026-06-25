using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class AccountDAO
    {
        private static AccountDAO? _instance;
        private static readonly object _lock = new();

        private AccountDAO() { }

        public static AccountDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AccountDAO();
                    }
                }
                return _instance;
            }
        }

        public IQueryable<Account> ForCompany(ExpenseDbContext context, int companyId) =>
            context.Accounts
                .AsNoTracking()
                .Where(a => a.CompanyId == companyId && a.IsActive);

        public async Task<Account?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Accounts.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

        public async Task<Account?> GetByIdForCompanyAsync(ExpenseDbContext context, int companyId, int id) =>
            await context.Accounts.FirstOrDefaultAsync(a => a.Id == id && a.CompanyId == companyId);
    }
}
