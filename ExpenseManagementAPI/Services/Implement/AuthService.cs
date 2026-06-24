using BusinessObjects.Extensions;
using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Auth;
using ExpenseManagementAPI.Services.Interface;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services.Implement
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<(bool Success, AuthResponse? Data, string? Error)> RegisterAsync(RegisterRequest request)
        {
            if (await _userRepository.UserNameExistsAsync(request.Username))
                return (false, null, "Tên đăng nhập đã tồn tại.");

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                await _userRepository.EmailExistsAsync(request.Email))
                return (false, null, "Email đã được sử dụng.");

            var user = new User
            {
                UserName = request.Username.Trim(),
                Password = _passwordHasher.Hash(request.Password),
                Email = request.Email?.Trim(),
                FullName = request.FullName?.Trim(),
                IsActive = true
            };

            await _userRepository.RegisterPersonalUserAsync(user);
            return (true, BuildAuthResponse(user), null);
        }

        public async Task<(bool Success, AuthResponse? Data, string? Error)> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUserNameAsync(request.Username.Trim());
            if (user == null)
                return (false, null, "Tên đăng nhập hoặc mật khẩu không đúng.");

            if (!user.IsActive)
                return (false, null, "Tài khoản đã bị khóa.");

            if (!_passwordHasher.Verify(request.Password, user.Password))
                return (false, null, "Tên đăng nhập hoặc mật khẩu không đúng.");

            return (true, BuildAuthResponse(user), null);
        }

        public async Task<(bool Success, UserProfileResponse? Data, string? Error)> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, null, "Không tìm thấy người dùng.");

            return (true, MapProfile(user), null);
        }

        public async Task<(bool Success, UserProfileResponse? Data, string? Error)> UpdateProfileAsync(
            int userId, UpdateProfileRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, null, "Không tìm thấy người dùng.");

            if (!string.IsNullOrWhiteSpace(request.Email) &&
                request.Email != user.Email &&
                await _userRepository.EmailExistsAsync(request.Email))
                return (false, null, "Email đã được sử dụng.");

            user.Email = request.Email?.Trim();
            user.FullName = request.FullName?.Trim();
            await _userRepository.UpdateAsync(user);
            return (true, MapProfile(user), null);
        }

        public async Task<(bool Success, string? Error)> ChangePasswordAsync(
            int userId, ChangePasswordRequest request)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "Không tìm thấy người dùng.");

            if (!_passwordHasher.Verify(request.CurrentPassword, user.Password))
                return (false, "Mật khẩu hiện tại không đúng.");

            user.Password = _passwordHasher.Hash(request.NewPassword);
            await _userRepository.UpdateAsync(user);
            return (true, null);
        }

        private static UserProfileResponse MapProfile(User user) => new()
        {
            UserId = user.Id,
            Username = user.UserName,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToClaimValue()
        };

        private AuthResponse BuildAuthResponse(User user) => new()
        {
            Token = _jwtTokenService.GenerateToken(user),
            UserId = user.Id,
            Username = user.UserName,
            Role = user.Role.ToClaimValue(),
            FullName = user.FullName,
            CompanyId = user.CompanyId
        };
    }
}
