using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [StringLength(100)]
        public string? Branch { get; set; }

        public int? OwnerUserId { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        public int? CompanyId { get; set; }

        [StringLength(50)]
        public string? Status { get; set; }

        [ForeignKey(nameof(OwnerUserId))]
        public User? OwnerUser { get; set; }

        [ForeignKey(nameof(CompanyId))]
        public Company? Company { get; set; }

        public ICollection<Budget> Budgets { get; set; } = new List<Budget>();
    }
}
