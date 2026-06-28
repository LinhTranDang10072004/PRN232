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

        public IQueryable<Account> ForCompanyManage(ExpenseDbContext context, int companyId) =>
            context.Accounts.AsNoTracking().Where(a => a.CompanyId == companyId);

        public async Task<bool> AccountNumberExistsAsync(
            ExpenseDbContext context, int companyId, string accountNumber, int? excludeId = null)
        {
            var normalized = accountNumber.Trim();
            return await context.Accounts.AnyAsync(a =>
                a.CompanyId == companyId &&
                a.AccountNumber == normalized &&
                (excludeId == null || a.Id != excludeId));
        }

        public async Task AddAsync(ExpenseDbContext context, Account account)
        {
            await context.Accounts.AddAsync(account);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, Account account)
        {
            context.Accounts.Update(account);
            await context.SaveChangesAsync();
        }
    }
}
