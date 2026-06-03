using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.DAOs
{
    public sealed class AppUserDAO
    {
        private static AppUserDAO? _instance;
        private static readonly object _lock = new();

        private AppUserDAO() { }

        public static AppUserDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        _instance ??= new AppUserDAO();
                    }
                }
                return _instance;
            }
        }

        public async Task<AppUser?> GetByIdAsync(ExpenseDbContext context, int id) =>
            await context.AppUsers.FindAsync(id);

        public async Task<AppUser?> GetByUsernameAsync(ExpenseDbContext context, string username) =>
            await context.AppUsers.FirstOrDefaultAsync(u => u.Username == username);

        public async Task<bool> UsernameExistsAsync(ExpenseDbContext context, string username) =>
            await context.AppUsers.AnyAsync(u => u.Username == username);

        public async Task<bool> EmailExistsAsync(ExpenseDbContext context, string email) =>
            await context.AppUsers.AnyAsync(u => u.Email == email);

        public async Task AddAsync(ExpenseDbContext context, AppUser user)
        {
            await context.AppUsers.AddAsync(user);
            await context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseDbContext context, AppUser user)
        {
            context.AppUsers.Update(user);
            await context.SaveChangesAsync();
        }

        /// <summary>Admin xem danh sách Staff do mình cấp tài khoản.</summary>
        public async Task<List<AppUser>> GetStaffByAdminAsync(ExpenseDbContext context, int adminId) =>
            await context.AppUsers
                .Where(u => u.Role == UserRole.Staff && u.ParentAdminId == adminId)
                .AsNoTracking()
                .ToListAsync();
    }
}
