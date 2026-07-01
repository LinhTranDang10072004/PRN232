using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace ExpenseManagementAPI.DTOs.Personal
{
    public class ExpenseRequest
    {
        [Required]
        [StringLength(255)]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int WalletId { get; set; }

        /// <summary>Tùy chọn — nếu null thì tự gắn theo Category + Ví + tháng/năm ExpenseDate.</summary>
        public int? BudgetDetailId { get; set; }
    }

    public class ExpenseResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime? ExpenseDate { get; set; }
        public ExpenseStatus Status { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public int? WalletId { get; set; }
        public string? WalletName { get; set; }
        public int? BudgetDetailId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool BudgetExceeded { get; set; }
        public decimal BudgetOverflowAmount { get; set; }
    }
}
