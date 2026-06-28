using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using DataAccessObjects.Context;
using DataAccessObjects.DAOs;
using Repositories.Interfaces;

namespace Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ExpenseDbContext _context;

        public UserRepository(ExpenseDbContext context)
        {
            _context = context;
        }

        public Task<User?> GetByIdAsync(int id) =>
            UserDAO.Instance.GetByIdAsync(_context, id);

        public Task<User?> GetByUserNameAsync(string userName) =>
            UserDAO.Instance.GetByUserNameAsync(_context, userName);

        public Task<User?> GetByEmailAsync(string email) =>
            UserDAO.Instance.GetByEmailAsync(_context, email);

        public async Task<User?> GetByLoginAsync(string login)
        {
            var trimmed = login.Trim();
            var byUserName = await GetByUserNameAsync(trimmed);
            if (byUserName != null)
                return byUserName;

            return await GetByEmailAsync(trimmed);
        }

        public Task<bool> UserNameExistsAsync(string userName) =>
            UserDAO.Instance.UserNameExistsAsync(_context, userName);

        public Task<bool> EmailExistsAsync(string email) =>
            UserDAO.Instance.EmailExistsAsync(_context, email);

        public Task RegisterPersonalUserAsync(User user)
        {
            user.Role = UserRole.User;
            user.CompanyId = null;
            return UserDAO.Instance.AddAsync(_context, user);
        }

        public async Task<User> CreateStaffAsync(User staff, int companyId)
        {
            staff.Role = UserRole.CompanyStaff;
            staff.CompanyId = companyId;

            var (ok, error) = UserAccountRules.ValidateCompanyId(staff.Role, staff.CompanyId);
            if (!ok)
                throw new InvalidOperationException(error);

            await UserDAO.Instance.AddAsync(_context, staff);
            return staff;
        }

        public Task<List<User>> GetStaffByCompanyAsync(int companyId) =>
            UserDAO.Instance.GetStaffByCompanyAsync(_context, companyId);

        public Task<User?> GetStaffInCompanyAsync(int companyId, int staffId) =>
            UserDAO.Instance.GetStaffInCompanyAsync(_context, companyId, staffId);

        public Task UpdateAsync(User user) =>
            UserDAO.Instance.UpdateAsync(_context, user);
    }
}
