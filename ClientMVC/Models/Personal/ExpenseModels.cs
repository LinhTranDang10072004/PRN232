using System.ComponentModel.DataAnnotations;
using BusinessObjects.Validation;
using ClientMVC.Models.Shared;

namespace ClientMVC.Models.Personal
{
    public class ExpenseItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Status { get; set; } = null!;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = null!;
    }

    public class ExpenseFormModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nhập tên khoản chi")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [NotFutureDate]
        public DateTime ExpenseDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Chọn danh mục")]
        public int CategoryId { get; set; }
    }

    public class PersonalCalendarDayCell
    {
        public int? Day { get; set; }
        public int ExpenseCount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsToday { get; set; }
        public bool IsSelected { get; set; }
    }

    public class PersonalCalendarViewModel
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int SelectedDay { get; set; }
        public string MaxDate => DateTime.Today.ToString("yyyy-MM-dd");
        public CalendarFilterModel Filters { get; set; } = new();
        public string MonthTitle => $"Tháng {Month}/{Year}";
        public List<PersonalCalendarDayCell> CalendarDays { get; set; } = new();
        public List<ExpenseItem> DayExpenses { get; set; } = new();
        public List<ExpenseItem> MonthExpenses { get; set; } = new();
        public List<CategoryItem> Categories { get; set; } = new();
        public ExpenseFormModel Form { get; set; } = new();
        public bool ShowFormModal { get; set; }
        public bool IsEdit { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
    }

    public class ExpenseIndexViewModel
    {
        public List<ExpenseItem> Items { get; set; } = new();
        public List<CategoryItem> Categories { get; set; } = new();
        public ExpenseFormModel Form { get; set; } = new();
        public string? FilterKeyword { get; set; }
        public decimal? FilterMinAmount { get; set; }
        public int? FilterCategoryId { get; set; }
        public string? ErrorMessage { get; set; }
        public string? SuccessMessage { get; set; }
        public bool ShowFormModal { get; set; }
        public bool IsEdit { get; set; }
    }
}
