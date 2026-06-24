using ExpenseManagementAPI.DTOs.Staff;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class StaffCategoryService : IStaffCategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public StaffCategoryService(ICategoryRepository categoryRepository) =>
            _categoryRepository = categoryRepository;

        public IQueryable<StaffCategoryResponse> GetForCompany(int companyId) =>
            _categoryRepository.GetForCorporateCompany(companyId).Select(c => new StaffCategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Status = c.Status
            });
    }
}
