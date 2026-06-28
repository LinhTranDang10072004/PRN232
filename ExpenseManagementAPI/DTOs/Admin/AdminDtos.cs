using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;
using ExpenseManagementAPI.DTOs.Staff;

namespace ExpenseManagementAPI.DTOs.Admin
{
    public class AdminExpenseResponse : StaffExpenseResponse
    {
        public int? StaffUserId { get; set; }
        public string? StaffUserName { get; set; }
        public string? StaffFullName { get; set; }
    }

    public class RejectExpenseRequest
    {
        [Required(ErrorMessage = "Nhập lý do từ chối")]
        [StringLength(500)]
        public string Comment { get; set; } = null!;
    }

    public class AdminDashboardResponse
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal TotalPendingAmount { get; set; }
        public int StaffCount { get; set; }
    }

    public class CreateStaffRequest
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }
    }

    public class StaffUserResponse
    {
        public int Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateStaffStatusRequest
    {
        public bool IsActive { get; set; }
    }

    public class AdminCategoryRequest
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        public CategoryStatus Status { get; set; } = CategoryStatus.Active;
    }

    public class AdminCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryStatus Status { get; set; }
    }

    public class AdminAccountRequest
    {
        [StringLength(255)]
        public string? Name { get; set; }

        [Required]
        [StringLength(100)]
        public string AccountNumber { get; set; } = null!;

        public bool IsActive { get; set; } = true;
    }

    public class AdminAccountResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string AccountNumber { get; set; } = null!;
        public bool IsActive { get; set; }
    }
}
