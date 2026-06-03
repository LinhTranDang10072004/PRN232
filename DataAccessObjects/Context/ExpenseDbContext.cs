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

        public DbSet<AppUser> AppUsers => Set<AppUser>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Expense> Expenses => Set<Expense>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Username).HasMaxLength(50);
                entity.Property(u => u.Email).HasMaxLength(256);
                entity.Property(u => u.FullName).HasMaxLength(100);

                entity.HasOne(u => u.ParentAdmin)
                    .WithMany(u => u.Staffs)
                    .HasForeignKey(u => u.ParentAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(c => c.Name).HasMaxLength(100);
                entity.HasIndex(c => new { c.Name, c.Branch, c.OwnerUserId })
                    .IsUnique()
                    .HasFilter("[OwnerUserId] IS NOT NULL");

                entity.HasIndex(c => new { c.Name, c.Branch })
                    .IsUnique()
                    .HasFilter("[OwnerUserId] IS NULL");

                entity.HasOne(c => c.OwnerUser)
                    .WithMany()
                    .HasForeignKey(c => c.OwnerUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Expense>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.RejectionReason).HasMaxLength(500);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.HasIndex(e => e.UserId);
                entity.HasIndex(e => e.ExpenseDate);
                entity.HasIndex(e => e.Status);

                entity.HasOne(e => e.User)
                    .WithMany(u => u.Expenses)
                    .HasForeignKey(e => e.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.ReviewedByAdmin)
                    .WithMany()
                    .HasForeignKey(e => e.ReviewedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedCategories(modelBuilder);
        }

        private static void SeedCategories(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Ăn uống", Branch = CategoryBranch.Personal },
                new Category { Id = 2, Name = "Di chuyển", Branch = CategoryBranch.Personal },
                new Category { Id = 3, Name = "Mua sắm", Branch = CategoryBranch.Personal },
                new Category { Id = 4, Name = "Giải trí", Branch = CategoryBranch.Personal },
                new Category { Id = 5, Name = "Khác (cá nhân)", Branch = CategoryBranch.Personal },
                new Category { Id = 10, Name = "Công tác phí", Branch = CategoryBranch.Corporate },
                new Category { Id = 11, Name = "Văn phòng phẩm", Branch = CategoryBranch.Corporate },
                new Category { Id = 12, Name = "Tiếp khách", Branch = CategoryBranch.Corporate },
                new Category { Id = 13, Name = "Đi lại", Branch = CategoryBranch.Corporate },
                new Category { Id = 14, Name = "Khác (công ty)", Branch = CategoryBranch.Corporate }
            );
        }
    }
}
