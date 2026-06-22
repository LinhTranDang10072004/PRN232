using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Models.Auth;
using ClientMVC.Models.Corporate;

namespace ClientMVC.Services
{
    public class CorporateApiClient : ICorporateApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

        public CorporateApiClient(HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public Task<(List<CorporateExpenseItem>? Data, string? Error)> GetStaffExpensesAsync(string? odataQuery = null) =>
            GetListAsync<CorporateExpenseItem>($"api/corporate/staff/expenses{odataQuery}");

        public async Task<(CorporateExpenseItem? Data, string? Error)> GetStaffExpenseAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Get, $"api/corporate/staff/expenses/{id}");
            return await ReadSingleAsync<CorporateExpenseItem>(response);
        }

        public async Task<(List<CategoryOptionItem>? Data, string? Error)> GetStaffCategoriesAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/corporate/staff/expenses/categories");
            if (!response.IsSuccessStatusCode)
                return (null, await ReadErrorAsync(response));
            var list = await response.Content.ReadFromJsonAsync<List<CategoryOptionItem>>(JsonOptions);
            return (list, null);
        }

        public async Task<(CorporateExpenseItem? Data, string? Error)> CreateStaffExpenseAsync(CorporateExpenseFormModel model)
        {
            var response = await SendAsync(HttpMethod.Post, "api/corporate/staff/expenses", ToRequest(model));
            return await ReadSingleAsync<CorporateExpenseItem>(response);
        }

        public async Task<(CorporateExpenseItem? Data, string? Error)> UpdateStaffExpenseAsync(int id, CorporateExpenseFormModel model)
        {
            var response = await SendAsync(HttpMethod.Put, $"api/corporate/staff/expenses/{id}", ToRequest(model));
            return await ReadSingleAsync<CorporateExpenseItem>(response);
        }

        public async Task<(bool Success, string? Error)> DeleteStaffExpenseAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Delete, $"api/corporate/staff/expenses/{id}");
            return response.IsSuccessStatusCode ? (true, null) : (false, await ReadErrorAsync(response));
        }

        public async Task<(List<CategoryOptionItem>? Data, string? Error)> GetAdminCategoriesAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/corporate/admin/expenses/categories");
            if (!response.IsSuccessStatusCode)
                return (null, await ReadErrorAsync(response));
            var list = await response.Content.ReadFromJsonAsync<List<CategoryOptionItem>>(JsonOptions);
            return (list, null);
        }

        public Task<(List<CorporateExpenseItem>? Data, string? Error)> GetAdminExpensesAsync(string? odataQuery = null) =>
            GetListAsync<CorporateExpenseItem>($"api/corporate/admin/expenses{odataQuery}");

        public async Task<(bool Success, string? Error)> ApproveExpenseAsync(int id)
        {
            var response = await SendAsync(HttpMethod.Post, $"api/corporate/admin/expenses/{id}/approve");
            return response.IsSuccessStatusCode ? (true, null) : (false, await ReadErrorAsync(response));
        }

        public async Task<(bool Success, string? Error)> RejectExpenseAsync(int id, string reason)
        {
            var response = await SendAsync(HttpMethod.Post, $"api/corporate/admin/expenses/{id}/reject", new { Reason = reason });
            return response.IsSuccessStatusCode ? (true, null) : (false, await ReadErrorAsync(response));
        }

        public async Task<(List<StaffMemberItem>? Data, string? Error)> GetStaffsAsync()
        {
            var response = await SendAsync(HttpMethod.Get, "api/corporate/admin/staffs");
            if (!response.IsSuccessStatusCode)
                return (null, await ReadErrorAsync(response));
            var list = await response.Content.ReadFromJsonAsync<List<StaffMemberItem>>(JsonOptions);
            return (list, null);
        }

        public async Task<(StaffMemberItem? Data, string? Error)> CreateStaffAsync(CreateStaffFormModel model)
        {
            var response = await SendAsync(HttpMethod.Post, "api/corporate/admin/staffs", model);
            return await ReadSingleAsync<StaffMemberItem>(response);
        }

        public async Task<(bool Success, string? Message, string? Error)> SetStaffActiveAsync(int staffId, bool isActive)
        {
            var response = await SendAsync(HttpMethod.Put, $"api/corporate/admin/staffs/{staffId}/active", new { IsActive = isActive });
            if (!response.IsSuccessStatusCode)
                return (false, null, await ReadErrorAsync(response));

            try
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return (true, msg.GetString(), null);
            }
            catch { }

            return (true, isActive ? "Đã kích hoạt nhân viên." : "Đã vô hiệu hóa nhân viên.", null);
        }

        private static object ToRequest(CorporateExpenseFormModel m) => new
        {
            m.Title,
            m.Description,
            m.Amount,
            m.ExpenseDate,
            m.CategoryId
        };

        private async Task<(List<T>? Data, string? Error)> GetListAsync<T>(string url)
        {
            var response = await SendAsync(HttpMethod.Get, url);
            if (!response.IsSuccessStatusCode)
                return (null, await ReadErrorAsync(response));

            var json = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.ValueKind == JsonValueKind.Array)
                    return (JsonSerializer.Deserialize<List<T>>(json, JsonOptions), null);
                if (doc.RootElement.TryGetProperty("value", out var value))
                    return (JsonSerializer.Deserialize<List<T>>(value.GetRawText(), JsonOptions), null);
            }
            catch
            {
                return (null, "Không đọc được dữ liệu từ API.");
            }
            return (new List<T>(), null);
        }

        private static async Task<(T? Data, string? Error)> ReadSingleAsync<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return (await response.Content.ReadFromJsonAsync<T>(JsonOptions), null);
            return (default, await ReadErrorAsync(response));
        }

        private async Task<HttpResponseMessage> SendAsync(HttpMethod method, string url, object? body = null)
        {
            var token = _httpContextAccessor.HttpContext?.User.FindFirst("jwt")?.Value;
            if (string.IsNullOrEmpty(token))
                throw new InvalidOperationException("Phiên đăng nhập hết hạn.");

            using var request = new HttpRequestMessage(method, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            if (body != null)
                request.Content = JsonContent.Create(body);
            return await _httpClient.SendAsync(request);
        }

        private static async Task<string?> ReadErrorAsync(HttpResponseMessage response)
        {
            try
            {
                var error = await response.Content.ReadFromJsonAsync<ApiErrorResponse>();
                if (!string.IsNullOrWhiteSpace(error?.Message))
                    return error.Message;
            }
            catch { }
            return $"Lỗi API ({(int)response.StatusCode})";
        }
    }
}
