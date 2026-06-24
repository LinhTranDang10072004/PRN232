using BusinessObjects.Enums;
using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects.Context
{
    public class ExpenseDbContext : DbContext
    {
        public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options)
        {
        }

        public DbSet<Company> Companies => Set<Company>();
        public DbSet<User> Users => Set<User>();
        public DbSet<Account> Accounts => Set<Account>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Notification> Notifications => Set<Notification>();
        public DbSet<Wallet> Wallets => Set<Wallet>();
        public DbSet<Budget> Budgets => Set<Budget>();
        public DbSet<BudgetDetail> BudgetDetails => Set<BudgetDetail>();
        public DbSet<Expense> Expenses => Set<Expense>();
        public DbSet<ApprovalHistory> ApprovalHistories => Set<ApprovalHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Company>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(255);
                entity.Property(c => c.Address).HasMaxLength(500);
                entity.Property(c => c.Status).HasMaxLength(50);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(u => u.UserName).HasMaxLength(100);
                entity.Property(u => u.Password).HasMaxLength(255);
                entity.Property(u => u.Email).HasMaxLength(100);
                entity.Property(u => u.FullName).HasMaxLength(255);
                entity.Property(u => u.Role)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasIndex(u => u.UserName).IsUnique();
                entity.HasIndex(u => new { u.CompanyId, u.Role });

                entity.HasOne(u => u.Company)
                    .WithMany(c => c.Users)
                    .HasForeignKey(u => u.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Account>(entity =>
            {
                entity.Property(a => a.Name).HasMaxLength(255);
                entity.Property(a => a.AccountNumber).HasMaxLength(100);

                entity.HasOne(a => a.Company)
                    .WithMany(c => c.Accounts)
                    .HasForeignKey(a => a.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(255);
                entity.Property(c => c.Type).HasMaxLength(50);
                entity.Property(c => c.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasIndex(c => new { c.Name, c.CompanyId, c.OwnerUserId });

                entity.HasOne(c => c.OwnerUser)
                    .WithMany(u => u.OwnedCategories)
                    .HasForeignKey(c => c.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(c => c.Company)
                    .WithMany(c => c.Categories)
                    .HasForeignKey(c => c.CompanyId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(n => n.Title).HasMaxLength(255);

                entity.HasOne(n => n.User)
                    .WithMany(u => u.Notifications)
                    .HasForeignKey(n => n.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Wallet>(entity =>
            {
                entity.Property(w => w.Name).HasMaxLength(255);
                entity.Property(w => w.Type).HasMaxLength(50);
                entity.Property(w => w.Status).HasMaxLength(50);
                entity.Property(w => w.Balance).HasColumnType("decimal(18,2)");

                entity.HasOne(w => w.User)
                    .WithMany(u => u.Wallets)
                    .HasForeignKey(w => w.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Budget>(entity =>
            {
                entity.Property(b => b.Name).HasMaxLength(255);
                entity.Property(b => b.Status).HasMaxLength(50);

                entity.HasOne(b => b.User)
                    .WithMany(u => u.Budgets)
                    .HasForeignKey(b => b.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(b => b.Category)
                    .WithMany(c => c.Budgets)
                    .HasForeignKey(b => b.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<BudgetDetail>(entity =>
            {
                entity.Property(d => d.LimitAmount).HasColumnType("decimal(18,2)");
                entity.Property(d => d.CurrentAmount).HasColumnType("decimal(18,2)");
                entity.Property(d => d.Status).HasMaxLength(50);

                entity.HasOne(d => d.Budget)
                    .WithMany(b => b.Details)
                    .HasForeignKey(d => d.BudgetId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(255);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Expenses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Category)
                    .WithMany(c => c.Expenses)
                    .HasForeignKey(e => e.CategoryId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.BudgetDetail)
                    .WithMany(d => d.Expenses)
                    .HasForeignKey(e => e.BudgetDetailId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Wallet)
                    .WithMany(w => w.Expenses)
                    .HasForeignKey(e => e.WalletId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Account)
                    .WithMany(a => a.Expenses)
                    .HasForeignKey(e => e.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<ApprovalHistory>(entity =>
            {
                entity.Property(h => h.Action).HasMaxLength(100);

                entity.HasOne(h => h.Expense)
                    .WithMany(e => e.ApprovalHistories)
                    .HasForeignKey(h => h.ExpenseId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.Admin)
                    .WithMany(u => u.ApprovalActions)
                    .HasForeignKey(h => h.AdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(h => h.Account)
                    .WithMany(a => a.ApprovalHistories)
                    .HasForeignKey(h => h.AccountId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedPersonalCategories(modelBuilder);
        }

        private static void SeedPersonalCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ăn uống", Status = CategoryStatus.Active, CompanyId = null, OwnerUserId = null },
                new Category { Id = 2, Name = "Di chuyển", Status = CategoryStatus.Active, CompanyId = null, OwnerUserId = null },
                new Category { Id = 3, Name = "Mua sắm", Status = CategoryStatus.Active, CompanyId = null, OwnerUserId = null },
                new Category { Id = 4, Name = "Giải trí", Status = CategoryStatus.Active, CompanyId = null, OwnerUserId = null },
                new Category { Id = 5, Name = "Khác (cá nhân)", Status = CategoryStatus.Active, CompanyId = null, OwnerUserId = null }
            );
        }
    }
}
