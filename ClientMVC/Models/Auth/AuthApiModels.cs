namespace ClientMVC.Models.Auth
{
    public class AuthApiResponse
    {
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string? FullName { get; set; }
    }

    public class ApiErrorResponse
    {
        public string? Message { get; set; }
    }
}
