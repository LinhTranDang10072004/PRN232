namespace ClientMVC.Models
{
    public class AuthResult
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? FullName { get; set; }
        public int? CompanyId { get; set; }
    }
}
