using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Auth
{
    public class ChangePasswordRequest
    {
        [Required]
        public string CurrentPassword { get; set; } = null!;

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string NewPassword { get; set; } = null!;
    }

    public class UpdateProfileRequest
    {
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? FullName { get; set; }
    }

    public class UserProfileResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string Role { get; set; } = null!;
    }
}
