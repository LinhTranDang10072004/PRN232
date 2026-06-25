using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class PersonalWalletService : IPersonalWalletService
    {
        private readonly IWalletRepository _walletRepository;

        public PersonalWalletService(IWalletRepository walletRepository) => _walletRepository = walletRepository;

        public IQueryable<WalletResponse> GetForUser(int userId) =>
            _walletRepository.GetForPersonalUser(userId)
                .Where(w => w.Status != "Inactive")
                .Select(w => new WalletResponse
                {
                    Id = w.Id,
                    Name = w.Name,
                    Type = w.Type,
                    Balance = w.Balance,
                    Status = w.Status
                });

        public async Task<(bool Success, WalletResponse? Data, string? Error)> GetByIdAsync(int userId, int id)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, id);
            return wallet == null
                ? (false, null, "Không tìm thấy ví.")
                : (true, Map(wallet), null);
        }

        public async Task<(bool Success, WalletResponse? Data, string? Error)> CreateAsync(
            int userId, WalletRequest request)
        {
            if (!WalletTypes.All.Contains(request.Type))
                return (false, null, "Loại ví không hợp lệ.");

            if (string.IsNullOrWhiteSpace(request.Name))
                return (false, null, "Tên ví không được rỗng.");

            if (request.InitialBalance < 0)
                return (false, null, "Số dư không được âm.");

            var wallet = new Wallet
            {
                UserId = userId,
                Name = request.Name.Trim(),
                Type = request.Type,
                Balance = request.InitialBalance,
                Status = "Active"
            };

            await _walletRepository.AddAsync(wallet);
            return (true, Map(wallet), null);
        }

        public async Task<(bool Success, WalletResponse? Data, string? Error)> UpdateAsync(
            int userId, int id, WalletRequest request)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, id);
            if (wallet == null)
                return (false, null, "Không tìm thấy ví.");

            if (wallet.Status == "Inactive")
                return (false, null, "Ví đã ngưng sử dụng.");

            if (!WalletTypes.All.Contains(request.Type))
                return (false, null, "Loại ví không hợp lệ.");

            wallet.Name = request.Name.Trim();
            wallet.Type = request.Type;
            await _walletRepository.UpdateAsync(wallet);
            return (true, Map(wallet), null);
        }

        public async Task<(bool Success, string? Error)> DeactivateAsync(int userId, int id)
        {
            var wallet = await _walletRepository.GetByIdForUserAsync(userId, id);
            if (wallet == null)
                return (false, "Không tìm thấy ví.");

            if (await _walletRepository.IsUsedByExpensesAsync(id))
            {
                wallet.Status = "Inactive";
                await _walletRepository.UpdateAsync(wallet);
                return (true, null);
            }

            wallet.Status = "Inactive";
            await _walletRepository.UpdateAsync(wallet);
            return (true, null);
        }

        private static WalletResponse Map(Wallet w) => new()
        {
            Id = w.Id,
            Name = w.Name,
            Type = w.Type,
            Balance = w.Balance,
            Status = w.Status
        };
    }
}
