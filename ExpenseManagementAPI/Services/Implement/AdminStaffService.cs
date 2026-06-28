using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class AdminStaffService : IAdminStaffService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;

        public AdminStaffService(IUserRepository userRepository, IPasswordHasherService passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<List<StaffUserResponse>> GetStaffAsync(int companyId)
        {
            var staff = await _userRepository.GetStaffByCompanyAsync(companyId);
            return staff.Select(Map).ToList();
        }

        public async Task<(bool Success, StaffUserResponse? Data, string? Error)> CreateStaffAsync(
            int companyId, CreateStaffRequest request)
        {
            if (await _userRepository.UserNameExistsAsync(request.UserName.Trim()))
                return (false, null, "Tên đăng nhập đã tồn tại.");

            if (await _userRepository.EmailExistsAsync(request.Email.Trim()))
                return (false, null, "Email đã được sử dụng.");

            var staff = new User
            {
                UserName = request.UserName.Trim(),
                Email = request.Email.Trim(),
                FullName = request.FullName?.Trim(),
                Password = _passwordHasher.Hash(request.Password),
                IsActive = true
            };

            var created = await _userRepository.CreateStaffAsync(staff, companyId);
            return (true, Map(created), null);
        }

        public async Task<(bool Success, string? Error)> UpdateStaffStatusAsync(
            int companyId, int staffId, bool isActive)
        {
            var staff = await _userRepository.GetStaffInCompanyAsync(companyId, staffId);
            if (staff == null)
                return (false, "Không tìm thấy nhân viên.");

            staff.IsActive = isActive;
            await _userRepository.UpdateAsync(staff);
            return (true, null);
        }

        private static StaffUserResponse Map(User u) => new()
        {
            Id = u.Id,
            UserName = u.UserName,
            Email = u.Email,
            FullName = u.FullName,
            IsActive = u.IsActive
        };
    }
}
