using System.ComponentModel.DataAnnotations;
using BusinessObjects.Enums;

namespace ClientMVC.Models
{
    public class ExpenseFilterModel
    {
        /// <summary>today | week | month | lastmonth | year | custom | all</summary>
        [Display(Name = "Khoảng thời gian")]
        public string Period { get; set; } = "all";

        [Display(Name = "Từ ngày")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "Đến ngày")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Danh mục")]
        public int? CategoryId { get; set; }

        [Display(Name = "Ví")]
        public int? WalletId { get; set; }

        [Display(Name = "Tài khoản")]
        public int? AccountId { get; set; }

        [Display(Name = "Trạng thái")]
        public ExpenseStatus? Status { get; set; }

        [Display(Name = "Số tiền tối thiểu")]
        public decimal? MinAmount { get; set; }

        [Display(Name = "Số tiền tối đa")]
        public decimal? MaxAmount { get; set; }

        [Display(Name = "Tìm theo tiêu đề")]
        public string? Keyword { get; set; }

        [Display(Name = "Tìm nhân viên")]
        public string? StaffKeyword { get; set; }
    }

    public class ExpenseFilterViewConfig
    {
        public bool ShowWallet { get; set; }
        public bool ShowAccount { get; set; }
        public bool ShowStatus { get; set; }
        public bool ShowStaffSearch { get; set; }
        public string FormAction { get; set; } = "Index";
        public string? Area { get; set; }
        public string? Controller { get; set; } = "Expenses";
    }

    public enum ExpenseFilterScope
    {
        Personal,
        Staff,
        Admin
    }
}
