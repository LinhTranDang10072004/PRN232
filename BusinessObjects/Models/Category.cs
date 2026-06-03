using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string Name { get; set; } = null!;

        [Required]
        public CategoryBranch Branch { get; set; }

        /// <summary>null = danh mục hệ thống (seed). Có giá trị = User tự tạo.</summary>
        public int? OwnerUserId { get; set; }

        public AppUser? OwnerUser { get; set; }

        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
