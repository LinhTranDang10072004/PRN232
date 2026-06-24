using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Personal
{
    public class WalletRequest
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        public string Type { get; set; } = null!;

        [Range(0, double.MaxValue)]
        public decimal InitialBalance { get; set; }
    }

    public class WalletResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Type { get; set; }
        public decimal Balance { get; set; }
        public string? Status { get; set; }
    }
}
