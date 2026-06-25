using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace ExpenseManagementAPI.DTOs.Staff
{
    public class StaffExpenseRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime? ExpenseDate { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int AccountId { get; set; }
    }

    public class StaffExpenseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public ExpenseStatus Status { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? AccountId { get; set; }
        public string? AccountName { get; set; }
        public string? AccountNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<ApprovalHistoryResponse> ApprovalHistories { get; set; } = new();
    }

    public class ApprovalHistoryResponse
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public string? Comment { get; set; }
        public string? AdminName { get; set; }
        public DateTime ActionDate { get; set; }
    }

    public class StaffCategoryResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryStatus Status { get; set; }
    }

    public class StaffAccountResponse
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string AccountNumber { get; set; } = null!;
    }

    public class StaffDashboardResponse
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal TotalPendingAmount { get; set; }
    }
}
