using BusinessObjects.Enums;
using System.ComponentModel.DataAnnotations;

namespace ClientMVC.Models.Admin
{
    public class AdminExpenseDto : Models.Staff.StaffExpenseDto
    {
        public int? StaffUserId { get; set; }
        public string? StaffUserName { get; set; }
        public string? StaffFullName { get; set; }
    }

    public class AdminDashboardDto
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public int StaffCount { get; set; }
    }

    public class RejectExpenseFormModel
    {
        [Required(ErrorMessage = "Nhập lý do từ chối")]
        [StringLength(500)]
        public string Comment { get; set; } = null!;
    }

    public class StaffUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateStaffFormModel
    {
        [Required(ErrorMessage = "Vui lòng nhập tên đăng nhập")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3–50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string UserName { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
        [Display(Name = "Họ và tên")]
        public string? FullName { get; set; }
    }

    public class AdminCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryStatus Status { get; set; }
    }

    public class AdminCategoryFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên danh mục")]
        [StringLength(255, ErrorMessage = "Tên danh mục tối đa 255 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string Name { get; set; } = null!;

        [Display(Name = "Trạng thái")]
        public CategoryStatus Status { get; set; } = CategoryStatus.Active;
    }

    public class AdminAccountDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string AccountNumber { get; set; } = null!;
        public bool IsActive { get; set; }
    }

    public class AdminAccountFormModel
    {
        public int? Id { get; set; }

        [StringLength(255, ErrorMessage = "Tên tài khoản tối đa 255 ký tự")]
        [Display(Name = "Tên hiển thị")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số tài khoản")]
        [StringLength(100, ErrorMessage = "Số tài khoản tối đa 100 ký tự")]
        [Display(Name = "Số tài khoản")]
        public string AccountNumber { get; set; } = null!;

        [Display(Name = "Đang hoạt động")]
        public bool IsActive { get; set; } = true;
    }
}
