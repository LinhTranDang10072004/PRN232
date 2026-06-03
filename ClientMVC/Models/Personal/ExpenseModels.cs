using System.ComponentModel.DataAnnotations;

namespace ClientMVC.Models.Personal
{
    public class ExpenseItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Status { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class ExpenseFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nhập tên khoản chi")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Chọn danh mục")]
        public int CategoryId { get; set; }
    }

    public class ExpenseIndexViewModel
    {
        public List<ExpenseItem> Items { get; set; } = new();
        public List<CategoryItem> Categories { get; set; } = new();
        public ExpenseFormModel Form { get; set; } = new();
        public string? FilterKeyword { get; set; }
        public decimal? FilterMinAmount { get; set; }
        public int? FilterCategoryId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool ShowFormModal { get; set; }
        public bool IsEdit { get; set; }
    }
}
