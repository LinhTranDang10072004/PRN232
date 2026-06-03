using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class AppUser
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3 - 50 ký tự")]
        public string Username { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(256)]
        public string Email { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }

        /// <summary>
        /// User = Nhánh 1 (tự đăng ký). Admin/Staff = Nhánh 2 (công ty).
        /// </summary>
        [Required]
        public UserRole Role { get; set; }

        /// <summary>
        /// Chỉ Staff: Id Admin đã cấp tài khoản (workspace công ty).
        /// User và Admin: null.
        /// </summary>
        public int? ParentAdminId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public AppUser? ParentAdmin { get; set; }
        public ICollection<AppUser> Staffs { get; set; } = new List<AppUser>();
        public ICollection<Expense> Expenses { get; set; } = new List<Expense>();
    }
}
