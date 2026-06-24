using BusinessObjects.Enums;
using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUserNameAsync(string userName);
        Task<bool> UserNameExistsAsync(string userName);
        Task<bool> EmailExistsAsync(string email);
        Task RegisterPersonalUserAsync(User user);
        Task<User> CreateStaffAsync(User staff, int companyId);
        Task<List<User>> GetStaffByCompanyAsync(int companyId);
        Task<User?> GetStaffInCompanyAsync(int companyId, int staffId);
        Task UpdateAsync(User user);
    }
}
