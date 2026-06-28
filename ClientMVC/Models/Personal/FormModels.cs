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

        [Required(ErrorMessage = "Vui lòng nhập tên ví")]
        [StringLength(255, ErrorMessage = "Tên ví tối đa 255 ký tự")]
        [Display(Name = "Tên ví")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn loại ví")]
        [Display(Name = "Loại ví")]
        public string Type { get; set; } = "Cash";

        [Display(Name = "Số dư ban đầu")]
        [Range(0, double.MaxValue, ErrorMessage = "Số dư ban đầu không được âm")]
        public decimal InitialBalance { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class CategoryFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(255, ErrorMessage = "Tên danh mục tối đa 255 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = null!;

        public string? ErrorMessage { get; set; }
    }

    public class BudgetFormModel
    {
        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Range(1, 12, ErrorMessage = "Tháng phải từ 1 đến 12")]
        [Display(Name = "Tháng")]
        public int Month { get; set; } = DateTime.Today.Month;

        [Range(2000, 2100, ErrorMessage = "Năm không hợp lệ")]
        [Display(Name = "Năm")]
        public int Year { get; set; } = DateTime.Today.Year;

        [Required(ErrorMessage = "Vui lòng nhập hạn mức")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Hạn mức phải lớn hơn 0")]
        [Display(Name = "Hạn mức")]
        public decimal LimitAmount { get; set; }

        [StringLength(255, ErrorMessage = "Tên budget tối đa 255 ký tự")]
        [Display(Name = "Tên budget")]
        public string? Name { get; set; }

        public string? ErrorMessage { get; set; }
    }

    public class ExpenseFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tiêu đề")]
        [StringLength(255, ErrorMessage = "Tiêu đề tối đa 255 ký tự")]
        [Display(Name = "Tiêu đề")]
        public string Title { get; set; } = null!;

        [Display(Name = "Mô tả")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tiền")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        [Display(Name = "Số tiền")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ngày chi")]
        [Display(Name = "Ngày chi")]
        [DataType(DataType.Date)]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục")]
        [Display(Name = "Danh mục")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn ví")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn ví")]
        [Display(Name = "Ví")]
        public int WalletId { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
