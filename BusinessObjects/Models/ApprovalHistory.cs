using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models
{
    [Table("ApprovalHistory")]
    public class ApprovalHistory
    {
        [Key]
        public int Id { get; set; }

        public int? ExpenseId { get; set; }

        public int? AdminId { get; set; }

        [StringLength(100)]
        public string? Action { get; set; }

        public string? Comment { get; set; }

        public DateTime ActionDate { get; set; } = DateTime.UtcNow;

        public int? AccountId { get; set; }

        [ForeignKey(nameof(ExpenseId))]
        public Expense? Expense { get; set; }

        [ForeignKey(nameof(AdminId))]
        public User? Admin { get; set; }

        [ForeignKey(nameof(AccountId))]
        public Account? Account { get; set; }
    }
}
