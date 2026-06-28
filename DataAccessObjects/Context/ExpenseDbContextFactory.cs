using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace DataAccessObjects.Context
{
    public class ExpenseDbContextFactory : IDesignTimeDbContextFactory<ExpenseDbContext>
    {
        public ExpenseDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "ExpenseManagementAPI"))
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            var optionsBuilder = new DbContextOptionsBuilder<ExpenseDbContext>();
            optionsBuilder.UseSqlServer(connectionString);
            return new ExpenseDbContext(optionsBuilder.Options);
        }
    }
}
