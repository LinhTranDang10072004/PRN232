using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Password { get; set; } = null!;

        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? FullName { get; set; }

        [StringLength(50)]
        public string? Role { get; set; }

        public int? ParentAdminId { get; set; }

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(ParentAdminId))]
        public User? ParentAdmin { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public ICollection<User> Staffs { get; set; } = new List<User>();
        public ICollection<Category> OwnedCategories { get; set; } = new List<Category>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<Wallet> Wallets { get; set; } = new List<Wallet>();
        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<ApprovalHistory> ApprovalActions { get; set; } = new List<ApprovalHistory>();
    }
}
