using System.ComponentModel.DataAnnotations;
using BusinessObjects.Validation;
using ClientMVC.Models.Shared;

namespace ClientMVC.Models.Corporate
{
    public class CorporateExpenseItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Status { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
        public int StaffId { get; set; }
        public string StaffUsername { get; set; } = null!;
        public string? StaffFullName { get; set; }
        public string? RejectionReason { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
    }

    public class CorporateExpenseFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nhập tên khoản chi")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [NotFutureDate]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required]
        public int CategoryId { get; set; }
    }

    public class CategoryOptionItem
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class CalendarDayCell
    {
        public int? Day { get; set; }
        public int ExpenseCount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsToday { get; set; }
        public bool IsSelected { get; set; }
        public bool HasPending { get; set; }
    }

    public class CorporateCalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int SelectedDay { get; set; }
        public string MaxDate => DateTime.Today.ToString("yyyy-MM-dd");
        public CalendarFilterModel Filters { get; set; } = new();
        public string MonthTitle => $"Tháng {Month}/{Year}";
        public List<CalendarDayCell> CalendarDays { get; set; } = new();
        public List<CorporateExpenseItem> DayExpenses { get; set; } = new();
        public List<CorporateExpenseItem> MonthExpenses { get; set; } = new();
        public List<CategoryOptionItem> Categories { get; set; } = new();
        public CorporateExpenseFormModel Form { get; set; } = new();
        public bool ShowFormModal { get; set; }
        public bool IsEdit { get; set; }
        public bool IsAdminView { get; set; }
        public int PendingCount { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public int? ReviewExpenseId { get; set; }
        public string RejectReason { get; set; } = string.Empty;
    }

    public class StaffMemberItem
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? FullName { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStaffFormModel
    {
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [StringLength(100)]
        public string? FullName { get; set; }
    }

    public class StaffManageViewModel
    {
        public List<StaffMemberItem> Staffs { get; set; } = new();
        public CreateStaffFormModel Form { get; set; } = new();
        public bool ShowCreateModal { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public int StaffCount { get; set; }
        public int PendingCount { get; set; }
        public int MonthExpenseCount { get; set; }
        public decimal MonthTotalAmount { get; set; }
    }

    public class StaffDashboardViewModel
    {
        public int PendingCount { get; set; }
        public int ApprovedCount { get; set; }
        public int RejectedCount { get; set; }
    }
}
