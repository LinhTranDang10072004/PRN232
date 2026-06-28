using System.ComponentModel.DataAnnotations;

namespace ExpenseManagementAPI.DTOs.Auth
{
    public class LoginRequest
    {
        /// <summary>Tên đăng nhập hoặc email.</summary>
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}
