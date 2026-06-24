using System.ComponentModel.DataAnnotations;

namespace ClientMVC.Models.Staff
{
    public class StaffExpenseFormModel
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
        [Display(Name = "Tài khoản công ty")]
        public int AccountId { get; set; }

        public string? ErrorMessage { get; set; }
        public bool IsLocked { get; set; }
    }

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
}
