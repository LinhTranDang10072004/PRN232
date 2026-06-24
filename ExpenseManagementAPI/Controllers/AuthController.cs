using ExpenseManagementAPI.DTOs.Auth;
using ExpenseManagementAPI.Helpers;
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

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, data, error) = await _authService.RegisterAsync(request);
            if (!success)
                return BadRequest(new { message = error });

            return Ok(data);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (success, data, error) = await _authService.LoginAsync(request);
            if (!success)
                return Unauthorized(new { message = error });

            return Ok(data);
        }

        [HttpGet("me")]
        [Authorize(Roles = "User,Admin,Staff")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.GetUserId();
            var (success, data, error) = await _authService.GetProfileAsync(userId);
            if (!success)
                return NotFound(new { message = error });
            return Ok(data);
        }

        [HttpPut("profile")]
        [Authorize(Roles = "User,Admin,Staff")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var (success, data, error) = await _authService.UpdateProfileAsync(userId, request);
            if (!success)
                return BadRequest(new { message = error });
            return Ok(data);
        }

        [HttpPut("change-password")]
        [Authorize(Roles = "User,Admin,Staff")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = User.GetUserId();
            var (success, error) = await _authService.ChangePasswordAsync(userId, request);
            if (!success)
                return BadRequest(new { message = error });
            return Ok(new { message = "Đổi mật khẩu thành công." });
        }
    }
}
