using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace ExpenseManagementAPI.DTOs.Personal
{
    public class CategoryRequest
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;
    }

    public class CategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryStatus Status { get; set; }
        public bool IsSystem { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }
}
