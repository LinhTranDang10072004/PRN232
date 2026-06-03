using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetByBranchAsync(CategoryBranch branch);
        Task<Category?> GetByIdAsync(int id);
    }
}
