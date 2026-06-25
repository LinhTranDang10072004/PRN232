using ExpenseManagementAPI.DTOs.Staff;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class StaffAccountService : IStaffAccountService
    {
        private readonly IAccountRepository _accountRepository;

        public StaffAccountService(IAccountRepository accountRepository) =>
            _accountRepository = accountRepository;

        public IQueryable<StaffAccountResponse> GetForCompany(int companyId) =>
            _accountRepository.GetForCompany(companyId).Select(a => new StaffAccountResponse
            {
                Id = a.Id,
                Name = a.Name,
                AccountNumber = a.AccountNumber
            });
    }
}
