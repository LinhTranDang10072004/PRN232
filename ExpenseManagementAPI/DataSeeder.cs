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
    }
}
