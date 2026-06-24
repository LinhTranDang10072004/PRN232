using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataAccessObjects.Context
{
    public class ExpenseDbContextFactory : IDesignTimeDbContextFactory<ExpenseDbContext>
    {
        public ExpenseDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ExpenseDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=127.0.0.1,1433;uid=sa;password=YourStrong!Passw0rd;database=ExpenseManagement;Encrypt=True;TrustServerCertificate=True;");
            return new ExpenseDbContext(optionsBuilder.Options);
        }
    }
}
