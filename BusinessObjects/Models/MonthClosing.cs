using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BusinessObjects.Models
{
    [Table("MonthClosing")]
    public class MonthClosing
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int Month { get; set; }

        [Required]
        public int Year { get; set; }

        public DateTime ClosedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSpent { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalBudgetLimit { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalSurplus { get; set; }

        public int BudgetCount { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Closed";

        [ForeignKey(nameof(UserId))]
        public User? User { get; set; }
    }
}
