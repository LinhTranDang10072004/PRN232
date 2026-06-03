using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BusinessObjects.Enums;

namespace BusinessObjects.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên khoản chi không được để trống")]
        [StringLength(200)]
        public string Title { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Số tiền không được để trống")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Số tiền phải lớn hơn 0")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Ngày chi tiêu không được để trống")]
        public DateTime ExpenseDate { get; set; }

        /// <summary>Nhánh 2: Pending/Approved/Rejected. Nhánh 1: API luôn ghi Approved.</summary>
        [Required]
        public ExpenseStatus Status { get; set; } = ExpenseStatus.Pending;

        /// <summary>User (cá nhân) hoặc Staff (công ty) — API filter theo Role.</summary>
        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUser User { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public Category Category { get; set; } = null!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? ReviewedAt { get; set; }

        public int? ReviewedByAdminId { get; set; }

        [ForeignKey(nameof(ReviewedByAdminId))]
        public AppUser? ReviewedByAdmin { get; set; }

        [StringLength(500)]
        public string? RejectionReason { get; set; }
    }
}
