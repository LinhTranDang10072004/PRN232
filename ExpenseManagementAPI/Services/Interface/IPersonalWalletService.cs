using ExpenseManagementAPI.DTOs.Personal;

namespace ExpenseManagementAPI.Services.Interface
{
    public interface IPersonalWalletService
    {
        IQueryable<WalletResponse> GetForUser(int userId);
        Task<(bool Success, WalletResponse? Data, string? Error)> GetByIdAsync(int userId, int id);
        Task<(bool Success, WalletResponse? Data, string? Error)> CreateAsync(int userId, WalletRequest request);
        Task<(bool Success, WalletResponse? Data, string? Error)> UpdateAsync(int userId, int id, WalletRequest request);
        Task<(bool Success, string? Error)> DeactivateAsync(int userId, int id);
    }
}
