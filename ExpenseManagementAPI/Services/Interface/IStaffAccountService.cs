using ExpenseManagementAPI.DTOs.Staff;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IStaffAccountService
    {
        IQueryable<StaffAccountResponse> GetForCompany(int companyId);
    }
}
