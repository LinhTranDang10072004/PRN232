using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [StringLength(50)]
        public string? Type { get; set; }

        /// <summary>Nhánh 1: ID người tạo. Nhánh 2 (công ty): NULL.</summary>
        public int? OwnerUserId { get; set; }

        /// <summary>Nhánh 1: NULL. Nhánh 2: ID công ty.</summary>
        public int? CompanyId { get; set; }

        public CategoryStatus Status { get; set; } = CategoryStatus.Active;

        [ForeignKey(nameof(OwnerUserId))]
        public User? OwnerUser { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
