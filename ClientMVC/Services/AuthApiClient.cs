using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Models;

namespace ClientMVC.Services
{
    public class AuthApiClient : IAuthApiClient
    {
        private readonly HttpClient _http;

        public AuthApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<(bool Success, AuthResult? Data, string? Error)> LoginAsync(LoginViewModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/login", new
            {
                username = model.Username,
                password = model.Password
            });

            return await ParseAuthResponseAsync(response);
        }

        public async Task<(bool Success, AuthResult? Data, string? Error)> RegisterAsync(RegisterViewModel model)
        {
            var response = await _http.PostAsJsonAsync("api/auth/register", new
            {
                username = model.Username,
                password = model.Password,
                email = model.Email,
                fullName = model.FullName
            });

            return await ParseAuthResponseAsync(response);
        }

        private static async Task<(bool Success, AuthResult? Data, string? Error)> ParseAuthResponseAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<AuthResult>();
                return data != null
                    ? (true, data, null)
                    : (false, null, "Phản hồi từ API không hợp lệ.");
            }

            var body = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var message))
                    return (false, null, message.GetString());
            }
            catch
            {
                // ignore parse errors
            }

            return (false, null, "Yêu cầu thất bại. Vui lòng thử lại.");
        }
    }
}
