using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagementAPI.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
            var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            await context.Database.MigrateAsync();

            var admin = await context.AppUsers.FirstOrDefaultAsync(u => u.Username == "admin");
            if (admin == null)
            {
                admin = new AppUser
                {
                    Username = "admin",
                    Email = "admin@company.com",
                    FullName = "Quản trị công ty",
                    Password = hasher.Hash("Admin@123"),
                    Role = UserRole.Admin,
                    ParentAdminId = null,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };
                context.AppUsers.Add(admin);
                await context.SaveChangesAsync();
            }
            else if (!hasher.Verify("Admin@123", admin.Password))
            {
                admin.Password = hasher.Hash("Admin@123");
                await context.SaveChangesAsync();
            }

            var staff = await context.AppUsers.FirstOrDefaultAsync(u => u.Username == "staff1");
            if (staff == null)
            {
                context.AppUsers.Add(new AppUser
                {
                    Username = "staff1",
                    Email = "staff1@company.com",
                    FullName = "Nhân viên mẫu",
                    Password = hasher.Hash("Staff@123"),
                    Role = UserRole.Staff,
                    ParentAdminId = admin.Id,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                });
                await context.SaveChangesAsync();
            }
            else if (!hasher.Verify("Staff@123", staff.Password))
            {
                staff.Password = hasher.Hash("Staff@123");
                staff.ParentAdminId ??= admin.Id;
                await context.SaveChangesAsync();
            }
        }
    }
}
