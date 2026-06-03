using System.ComponentModel.DataAnnotations;

namespace ClientMVC.Models.Personal
{
    public class CategoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Branch { get; set; } = null!;
        public bool IsSystem { get; set; }
        public int? OwnerUserId { get; set; }
    }

    public class CategoryFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nhập tên danh mục")]
        [StringLength(100)]
        public string Name { get; set; } = null!;
    }

    public class CategoryIndexViewModel
    {
        public List<CategoryItem> Items { get; set; } = new();
        public CategoryFormModel Form { get; set; } = new();
        public string? FilterKeyword { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool ShowFormModal { get; set; }
        public bool IsEdit { get; set; }
    }
}
