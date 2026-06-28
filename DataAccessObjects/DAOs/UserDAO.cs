using BusinessObjects.Enums;
using BusinessObjects.Models;
using BusinessObjects.Validation;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class UserDAO
    {
        private static UserDAO? _instance;
        private static readonly object _lock = new();

        private UserDAO() { }

        public static UserDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new UserDAO();
                    }
                }
                return _instance;
            }
        }

        public async Task<User?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.Users.FindAsync(id);

        public async Task<User?> GetByUserNameAsync(ExpenseDbContext context, string userName) =>
            await context.Users.FirstOrDefaultAsync(u => u.UserName == userName);

        public async Task<User?> GetByEmailAsync(ExpenseDbContext context, string email) =>
            await context.Users.FirstOrDefaultAsync(u => u.Email == email);

        public async Task<bool> UserNameExistsAsync(ExpenseDbContext context, string userName) =>
            await context.Users.AnyAsync(u => u.UserName == userName);

        public async Task<bool> EmailExistsAsync(ExpenseDbContext context, string email) =>
            await context.Users.AnyAsync(u => u.Email == email);

        public async Task AddAsync(ExpenseDbContext context, User user)
        {
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, User user)
        {
            context.Users.Update(user);
            await context.SaveChangesAsync();
        }

        /// <summary>Admin lấy Staff cùng CompanyId (không dùng ParentAdminId).</summary>
        public async Task<List<User>> GetStaffByCompanyAsync(ExpenseDbContext context, int companyId) =>
            await UserAccountRules.StaffOfCompany(context.Users, companyId)
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.UserName)
                .AsNoTracking()
                .ToListAsync();

        public async Task<User?> GetStaffInCompanyAsync(ExpenseDbContext context, int companyId, int staffId) =>
            await context.Users.FirstOrDefaultAsync(u =>
                u.Id == staffId &&
                u.Role == UserRole.CompanyStaff &&
                u.CompanyId == companyId);
    }
}
