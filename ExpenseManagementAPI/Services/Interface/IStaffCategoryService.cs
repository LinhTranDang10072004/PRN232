using ExpenseManagementAPI.DTOs.Staff;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IStaffCategoryService
    {
        IQueryable<StaffCategoryResponse> GetForCompany(int companyId);
    }
}
