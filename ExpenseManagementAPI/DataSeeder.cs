using BusinessObjects.Enums;
using BusinessObjects.Models;
using DataAccessObjects.Context;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace ExpenseManagementAPI
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ExpenseDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasherService>();

            await context.Database.MigrateAsync();

            await SeedPersonalUserAsync(context, passwordHasher);
            await SeedCorporateDemoAsync(context, passwordHasher);
        }

        private static async Task SeedPersonalUserAsync(ExpenseDbContext context, IPasswordHasherService passwordHasher)
        {
            if (await context.Users.AnyAsync(u => u.UserName == "user_demo"))
                return;

            context.Users.Add(new User
            {
                UserName = "user_demo",
                Password = passwordHasher.Hash("User@123"),
                Email = "user_demo@example.com",
                FullName = "Nguyễn Văn A",
                Role = UserRole.User,
                CompanyId = null,
                IsActive = true
            });

            await context.SaveChangesAsync();
        }

        private static async Task SeedCorporateDemoAsync(ExpenseDbContext context, IPasswordHasherService passwordHasher)
        {
            if (await context.Users.AnyAsync(u => u.UserName == "staff_demo"))
                return;

            var company = new Company
            {
                Name = "Công ty Demo ABC",
                Address = "123 Đường ABC, Quận 1, TP.HCM",
                Status = "Active"
            };
            context.Companies.Add(company);
            await context.SaveChangesAsync();

            context.Users.AddRange(
                new User
                {
                    UserName = "admin_demo",
                    Password = passwordHasher.Hash("Admin@123"),
                    Email = "admin@demo.com",
                    FullName = "Trần Quản Trị",
                    Role = UserRole.CompanyAdmin,
                    CompanyId = company.Id,
                    IsActive = true
                },
                new User
                {
                    UserName = "staff_demo",
                    Password = passwordHasher.Hash("Staff@123"),
                    Email = "staff@demo.com",
                    FullName = "Lê Nhân Viên",
                    Role = UserRole.CompanyStaff,
                    CompanyId = company.Id,
                    IsActive = true
                });

            context.Categories.AddRange(
                new Category { Name = "Công tác", Status = CategoryStatus.Active, CompanyId = company.Id },
                new Category { Name = "Văn phòng phẩm", Status = CategoryStatus.Active, CompanyId = company.Id },
                new Category { Name = "Đi lại", Status = CategoryStatus.Active, CompanyId = company.Id },
                new Category { Name = "Tiếp khách", Status = CategoryStatus.Active, CompanyId = company.Id },
                new Category { Name = "Khác (công ty)", Status = CategoryStatus.Active, CompanyId = company.Id });

            context.Accounts.AddRange(
                new Account
                {
                    Name = "Tài khoản công ty chính",
                    AccountNumber = "1234567890",
                    CompanyId = company.Id,
                    IsActive = true
                },
                new Account
                {
                    Name = "Quỹ chi nhỏ lẻ",
                    AccountNumber = "0987654321",
                    CompanyId = company.Id,
                    IsActive = true
                });

            await context.SaveChangesAsync();
        }
    }
}
