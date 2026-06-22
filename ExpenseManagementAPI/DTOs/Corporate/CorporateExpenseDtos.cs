using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Corporate
{
    public class CorporateExpenseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Status { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int StaffId { get; set; }
        public string StaffUsername { get; set; } = null!;
        public string? StaffFullName { get; set; }
        public string? RejectionReason { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class CorporateExpenseRequest
    {
        [Required(ErrorMessage = "Tên khoản chi không được để trống")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }

    public class RejectExpenseRequest
    {
        [Required(ErrorMessage = "Nhập lý do từ chối")]
        [StringLength(500)]
        public string Reason { get; set; } = null!;
    }

    public class CategoryOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }
}
