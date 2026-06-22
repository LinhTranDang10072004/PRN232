using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models
{
    [Table("BudgetDetail")]
    public class BudgetDetail
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal LimitAmount { get; set; }

        public int? BudgetId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal CurrentAmount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [ForeignKey(nameof(BudgetId))]
        public Budget? Budget { get; set; }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
