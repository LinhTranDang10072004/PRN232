using BusinessObjects.Enums;
using BusinessObjects.Models;
using ExpenseManagementAPI.DTOs.Auth;
using Repositories.Interfaces;

namespace ExpenseManagementAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAppUserRepository _userRepository;
        private readonly IPasswordHasherService _passwordHasher;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(
            IAppUserRepository userRepository,
            IPasswordHasherService passwordHasher,
            IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !user.IsActive)
                return null;

            if (!_passwordHasher.Verify(request.Password, user.Password))
                return null;

            return BuildResponse(user);
        }

        public async Task<(AuthResponse? Response, string? Error)> RegisterUserAsync(RegisterRequest request)
        {
            if (await _userRepository.UsernameExistsAsync(request.Username))
                return (null, "Tên đăng nhập đã tồn tại.");

            if (await _userRepository.EmailExistsAsync(request.Email))
                return (null, "Email đã được sử dụng.");

            var user = new AppUser
            {
                Username = request.Username.Trim(),
                Email = request.Email.Trim(),
                FullName = request.FullName?.Trim(),
                Password = _passwordHasher.Hash(request.Password),
                Role = UserRole.User,
                ParentAdminId = null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.RegisterPersonalUserAsync(user);
            return (BuildResponse(user), null);
        }

        private AuthResponse BuildResponse(AppUser user) => new()
        {
            Token = _jwtTokenService.GenerateToken(user),
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role.ToString(),
            FullName = user.FullName
        };
    }
}
