using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models
{
    [Table("Account")]
    public class Account
    {
        [Key]
        public int Id { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountNumber { get; set; } = null!;

        public int? CompanyId { get; set; }

        public bool IsActive { get; set; } = true;

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
        public ICollection<ApprovalHistory> ApprovalHistories { get; set; } = new List<ApprovalHistory>();
    }
}
