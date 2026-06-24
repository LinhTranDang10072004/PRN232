using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Personal
{
    public class CreateBudgetRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Range(1, 12)]
        public int Month { get; set; }

        [Range(2000, 2100)]
        public int Year { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal LimitAmount { get; set; }

        [StringLength(255)]
        public string? Name { get; set; }
    }

    public class UpdateBudgetLimitRequest
    {
        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal LimitAmount { get; set; }
    }

    public class BudgetResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public decimal LimitAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsExceeded { get; set; }
        public string? Status { get; set; }
    }
}
