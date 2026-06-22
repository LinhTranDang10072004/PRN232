using BusinessObjects.Models;

namespace Repositories.Interfaces
{
    public interface IAppUserRepository
    {
        Task<AppUser?> GetByIdAsync(int id);
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task RegisterPersonalUserAsync(AppUser user);
        Task<AppUser> CreateStaffAsync(AppUser staff, int adminId);
        Task<List<AppUser>> GetStaffByAdminAsync(int adminId);
        Task<AppUser?> GetStaffForAdminAsync(int adminId, int staffId);
        Task UpdateAsync(AppUser user);
    }
}
