using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class AdminAccountService : IAdminAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public AdminAccountService(IAccountRepository accountRepository) =>
            _accountRepository = accountRepository;

        public IQueryable<AdminAccountResponse> GetForCompany(int companyId) =>
            _accountRepository.GetForCompanyManage(companyId).Select(a => new AdminAccountResponse
            {
                Id = a.Id,
                Name = a.Name,
                AccountNumber = a.AccountNumber,
                IsActive = a.IsActive
            });

        public async Task<(bool Success, AdminAccountResponse? Data, string? Error)> CreateAsync(
            int companyId, AdminAccountRequest request)
        {
            var number = request.AccountNumber.Trim();
            if (await _accountRepository.AccountNumberExistsAsync(companyId, number))
                return (false, null, "Số tài khoản đã tồn tại.");

            var account = new Account
            {
                Name = request.Name?.Trim(),
                AccountNumber = number,
                CompanyId = companyId,
                IsActive = request.IsActive
            };

            await _accountRepository.AddAsync(account);
            return (true, Map(account), null);
        }

        public async Task<(bool Success, AdminAccountResponse? Data, string? Error)> UpdateAsync(
            int companyId, int id, AdminAccountRequest request)
        {
            var account = await _accountRepository.GetByIdForCompanyAsync(companyId, id);
            if (account == null)
                return (false, null, "Không tìm thấy tài khoản.");

            var number = request.AccountNumber.Trim();
            if (await _accountRepository.AccountNumberExistsAsync(companyId, number, id))
                return (false, null, "Số tài khoản đã tồn tại.");

            account.Name = request.Name?.Trim();
            account.AccountNumber = number;
            account.IsActive = request.IsActive;
            await _accountRepository.UpdateAsync(account);
            return (true, Map(account), null);
        }

        private static AdminAccountResponse Map(Account a) => new()
        {
            Id = a.Id,
            Name = a.Name,
            AccountNumber = a.AccountNumber,
            IsActive = a.IsActive
        };
    }
}
