using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models
{
    [Table("Expense")]
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime? ExpenseDate { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        public int? UserId { get; set; }

        public int? BudgetDetailId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? WalletId { get; set; }

        public int? AccountId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }

        [ForeignKey(nameof(BudgetDetailId))]
        public BudgetDetail? BudgetDetail { get; set; }

        [ForeignKey(nameof(WalletId))]
        public Wallet? Wallet { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }

        public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    }
}
