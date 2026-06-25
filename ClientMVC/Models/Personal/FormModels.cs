using System.ComponentModel.DataAnnotations;

namespace ClientMVC.Models.Personal
{
    public class ProfileFormModel
    {
        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    public class ChangePasswordFormModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu hiện tại")]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu mới")]
        public string NewPassword { get; set; } = null!;

        [Required]
        [Compare(nameof(NewPassword))]
        [DataType(DataType.Password)]
        [Display(Name = "Xác nhận mật khẩu mới")]
        public string ConfirmPassword { get; set; } = null!;

        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    public class WalletFormModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Tên ví")]
        public string Name { get; set; } = null!;

        [Required]
        [Display(Name = "Loại ví")]
        public string Type { get; set; } = "Cash";

        [Display(Name = "Số dư ban đầu")]
        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class CategoryFormModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = null!;

        public string? ErrorMessage { get; set; }
    }

    public class BudgetFormModel
    {
        [Required]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Range(1, 12)]
        [Display(Name = "Tháng")]
        public int Month { get; set; } = DateTime.Today.Month;

        [Range(2000, 2100)]
        [Display(Name = "Năm")]
        public int Year { get; set; } = DateTime.Today.Year;

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Hạn mức")]
        public decimal LimitAmount { get; set; }

        [Display(Name = "Tên budget")]
        public string? Name { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class ExpenseFormModel
    {
        public int? Id { get; set; }

        [Required]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Ngày chi")]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required]
        [Display(Name = "Ví")]
        public int WalletId { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
