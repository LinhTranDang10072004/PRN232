using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class AppUserRepository : IAppUserRepository
    {
        private readonly ExpenseDbContext _context;
        private readonly AppUserDAO _dao = AppUserDAO.Instance;

        public AppUserRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public Task<AppUser?> GetByIdAsync(int id) =>
            _dao.GetByIdAsync(_context, id);

        public Task<AppUser?> GetByUsernameAsync(string username) =>
            _dao.GetByUsernameAsync(_context, username);

        public Task<bool> UsernameExistsAsync(string username) =>
            _dao.UsernameExistsAsync(_context, username);

        public Task<bool> EmailExistsAsync(string email) =>
            _dao.EmailExistsAsync(_context, email);

        public async Task RegisterPersonalUserAsync(AppUser user)
        {
            user.Role = UserRole.User;
            user.ParentAdminId = null;
            await _dao.AddAsync(_context, user);
        }

        public async Task<AppUser> CreateStaffAsync(AppUser staff, int adminId)
        {
            staff.Role = UserRole.Staff;
            staff.ParentAdminId = adminId;
            await _dao.AddAsync(_context, staff);
            return staff;
        }

        public Task<List<AppUser>> GetStaffByAdminAsync(int adminId) =>
            _dao.GetStaffByAdminAsync(_context, adminId);

        public Task<AppUser?> GetStaffForAdminAsync(int adminId, int staffId) =>
            _dao.GetStaffForAdminAsync(_context, adminId, staffId);

        public Task UpdateAsync(AppUser user) =>
            _dao.UpdateAsync(_context, user);
    }
}
