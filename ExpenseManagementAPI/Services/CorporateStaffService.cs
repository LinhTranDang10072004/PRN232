using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Corporate;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services
{
    public class CorporateStaffService : ICorporateStaffService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public CorporateStaffService(
            IAppUserRepository userRepository,
            IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<StaffResponse>> GetStaffListAsync(int adminId)
        {
            var list = await _userRepository.GetStaffByAdminAsync(adminId);
            return list.Select(MapStaff).ToList();
        }

        public async Task<(StaffResponse? Result, string? Error)> CreateStaffAsync(
            int adminId, CreateStaffRequest request)
        {
            if (await _userRepository.UsernameExistsAsync(request.Username))
                return (null, "Tên đăng nhập đã tồn tại.");

            if (await _userRepository.EmailExistsAsync(request.Email))
                return (null, "Email đã được sử dụng.");

            var staff = new AppUser
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                FullName = request.FullName?.Trim(),
                Password = _passwordHasher.Hash(request.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _userRepository.CreateStaffAsync(staff, adminId);
            return (MapStaff(created), null);
        }

        public async Task<(StaffResponse? Result, string? Error)> SetStaffActiveAsync(
            int adminId, int staffId, bool isActive)
        {
            var staff = await _userRepository.GetStaffForAdminAsync(adminId, staffId);
            if (staff == null)
                return (null, "Không tìm thấy nhân viên trong workspace của bạn.");

            if (staff.IsActive == isActive)
                return (MapStaff(staff), null);

            staff.IsActive = isActive;
            await _userRepository.UpdateAsync(staff);

            var message = isActive
                ? "Đã kích hoạt lại tài khoản nhân viên."
                : "Đã vô hiệu hóa tài khoản (sa thải). Nhân viên không thể đăng nhập.";
            return (MapStaff(staff), message);
        }

        private static StaffResponse MapStaff(AppUser u) => new()
        {
            Id = u.Id,
            Username = u.Username,
            Email = u.Email,
            FullName = u.FullName,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        };
    }
}
