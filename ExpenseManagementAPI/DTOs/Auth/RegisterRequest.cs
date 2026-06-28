using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Username { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; } = null!;

        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
    }
}
