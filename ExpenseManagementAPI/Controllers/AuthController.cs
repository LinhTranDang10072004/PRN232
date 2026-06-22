using System.Security.Claims;
using ExpenseManagementAPI.DTOs.Auth;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (result, error) = await _authService.LoginAsync(request);
            if (result == null)
                return Unauthorized(new { message = error ?? "Tên đăng nhập hoặc mật khẩu không đúng." });

            return Ok(result);
        }

        /// <summary>Chỉ Nhánh 1: User tự đăng ký.</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (response, error) = await _authService.RegisterUserAsync(request);
            if (error != null)
                return BadRequest(new { message = error });

            return Ok(response);
        }

        [HttpGet("me")]
        [Authorize]
        public ActionResult<object> Me()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var username = User.FindFirstValue(ClaimTypes.Name);
            var role = User.FindFirstValue(ClaimTypes.Role);
            var email = User.FindFirstValue(ClaimTypes.Email);

            return Ok(new { userId, username, role, email });
        }
    }
}
