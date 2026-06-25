using BusinessObjects.Enums;

namespace ClientMVC.Models.Staff
{
    public class StaffExpenseDto
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
        public List<ApprovalHistoryDto> ApprovalHistories { get; set; } = new();
    }

    public class ApprovalHistoryDto
    {
        public int Id { get; set; }
        public string? Action { get; set; }
        public string? Comment { get; set; }
        public string? AdminName { get; set; }
        public DateTime ActionDate { get; set; }
    }

    public class StaffCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public CategoryStatus Status { get; set; }
    }

    public class StaffAccountDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string AccountNumber { get; set; } = null!;
    }

    public class StaffDashboardDto
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
        public decimal TotalPendingAmount { get; set; }
    }

    public class NotificationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Content { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class UserProfileDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = null!;
    }
}
