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
                "Server=LAPTOP-715LSPJN;uid=sa;password=123;database=ExpenseManagement;Encrypt=True;TrustServerCertificate=True;");
            return new ExpenseDbContext(optionsBuilder.Options);
        }
    }
}
