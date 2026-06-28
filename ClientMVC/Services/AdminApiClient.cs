using System.Net.Http.Json;
using System.Text.Json;
using ClientMVC.Models.Admin;

namespace ClientMVC.Services
{
    public class AdminApiClient : IAdminApiClient
    {
        private readonly HttpClient _http;

        public AdminApiClient(HttpClient http) => _http = http;

        public Task<AdminDashboardDto?> GetDashboardAsync() =>
            GetAsync<AdminDashboardDto>("api/admin/expenses/dashboard");

        public Task<List<AdminExpenseDto>> GetExpensesAsync(string? oDataFilter = null)
        {
            var url = "api/admin/expenses";
            if (!string.IsNullOrEmpty(oDataFilter))
                url += $"?$filter={Uri.EscapeDataString(oDataFilter)}";
            return GetODataListAsync<AdminExpenseDto>(url);
        }

        public Task<AdminExpenseDto?> GetExpenseAsync(int id) =>
            GetAsync<AdminExpenseDto>($"api/admin/expenses/{id}");

        public async Task<(bool Ok, string? Error)> ApproveExpenseAsync(int id)
        {
            var response = await _http.PostAsync($"api/admin/expenses/{id}/approve", null);
            return await ParseMessageAsync(response);
        }

        public async Task<(bool Ok, string? Error)> RejectExpenseAsync(int id, string comment)
        {
            var response = await _http.PostAsJsonAsync($"api/admin/expenses/{id}/reject",
                new { comment }, ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public Task<List<StaffUserDto>> GetStaffAsync() =>
            GetListAsync<StaffUserDto>("api/admin/staff");

        public async Task<(bool Ok, string? Error)> CreateStaffAsync(CreateStaffFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/admin/staff", new
            {
                userName = model.UserName,
                password = model.Password,
                email = model.Email,
                fullName = model.FullName
            }, ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public async Task<(bool Ok, string? Error)> UpdateStaffStatusAsync(int id, bool isActive)
        {
            var response = await _http.PutAsJsonAsync($"api/admin/staff/{id}/status",
                new { isActive }, ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public Task<List<AdminCategoryDto>> GetCategoriesAsync() =>
            GetODataListAsync<AdminCategoryDto>("api/admin/categories");

        public async Task<(bool Ok, string? Error)> CreateCategoryAsync(AdminCategoryFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/admin/categories", MapCategory(model), ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public async Task<(bool Ok, string? Error)> UpdateCategoryAsync(int id, AdminCategoryFormModel model)
        {
            var response = await _http.PutAsJsonAsync($"api/admin/categories/{id}", MapCategory(model), ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public Task<(bool Ok, string? Error)> DeleteCategoryAsync(int id) =>
            DeleteAsync($"api/admin/categories/{id}");

        public Task<List<AdminAccountDto>> GetAccountsAsync() =>
            GetODataListAsync<AdminAccountDto>("api/admin/accounts");

        public async Task<(bool Ok, string? Error)> CreateAccountAsync(AdminAccountFormModel model)
        {
            var response = await _http.PostAsJsonAsync("api/admin/accounts", MapAccount(model), ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        public async Task<(bool Ok, string? Error)> UpdateAccountAsync(int id, AdminAccountFormModel model)
        {
            var response = await _http.PutAsJsonAsync($"api/admin/accounts/{id}", MapAccount(model), ApiJsonOptions.Default);
            return await ParseMessageAsync(response);
        }

        private static object MapCategory(AdminCategoryFormModel model) => new
        {
            name = model.Name,
            status = model.Status
        };

        private static object MapAccount(AdminAccountFormModel model) => new
        {
            name = model.Name,
            accountNumber = model.AccountNumber,
            isActive = model.IsActive
        };

        private async Task<T?> GetAsync<T>(string url) where T : class
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return null;
            return await response.Content.ReadFromJsonAsync<T>(ApiJsonOptions.Default);
        }

        private async Task<List<T>> GetListAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<T>();
            return await response.Content.ReadFromJsonAsync<List<T>>(ApiJsonOptions.Default) ?? new List<T>();
        }

        private async Task<List<T>> GetODataListAsync<T>(string url)
        {
            var response = await _http.GetAsync(url);
            if (!response.IsSuccessStatusCode) return new List<T>();
            var body = await response.Content.ReadAsStringAsync();
            if (body.TrimStart().StartsWith('['))
                return JsonSerializer.Deserialize<List<T>>(body, ApiJsonOptions.Default) ?? new List<T>();
            var odata = JsonSerializer.Deserialize<ODataList<T>>(body, ApiJsonOptions.Default);
            return odata?.Value ?? new List<T>();
        }

        private static async Task<(bool Ok, string? Error)> ParseMessageAsync(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ReadErrorAsync(response));
        }

        private async Task<(bool Ok, string? Error)> DeleteAsync(string url)
        {
            var response = await _http.DeleteAsync(url);
            return await ParseMessageAsync(response);
        }

        private static async Task<string> ReadErrorAsync(HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(body);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                    return msg.GetString() ?? "Yêu cầu thất bại.";
            }
            catch { /* ignore */ }
            return "Yêu cầu thất bại.";
        }
    }
}
