using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Personal
{
    public class CreateCategoryRequest
    {
        [Required(ErrorMessage = "Tên danh mục không được để trống")]
        [StringLength(100)]
        public string Name { get; set; } = null!;
    }
}
