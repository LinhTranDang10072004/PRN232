using System.Security.Claims;
using ClientMVC.Models.Auth;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthApiClient _authApiClient;

        public AuthController(IAuthApiClient authApiClient)
        {
            _authApiClient = authApiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();

            ViewData["ReturnUrl"] = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (data, error) = await _authApiClient.LoginAsync(model);
            if (data == null)
            {
                ModelState.AddModelError(string.Empty, error ?? "Đăng nhập thất bại.");
                return View(model);
            }

            await SignInAsync(data);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToDashboard(data.Role);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToDashboard();

            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (data, error) = await _authApiClient.RegisterAsync(model);
            if (data == null)
            {
                ModelState.AddModelError(string.Empty, error ?? "Đăng ký thất bại.");
                return View(model);
            }

            await SignInAsync(data);
            return RedirectToPersonalExpenses();
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        private async Task SignInAsync(AuthApiResponse data)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, data.UserId.ToString()),
                new(ClaimTypes.Name, data.Username),
                new(ClaimTypes.Email, data.Email),
                new(ClaimTypes.Role, data.Role),
                new("jwt", data.Token)
            };

            if (!string.IsNullOrEmpty(data.FullName))
                claims.Add(new Claim("FullName", data.FullName));

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
                });
        }

        private IActionResult RedirectToDashboard(string? role = null)
        {
            role ??= User.FindFirstValue(ClaimTypes.Role);
            return role switch
            {
                "User" => RedirectToPersonalExpenses(),
                "Admin" => RedirectToAction("Dashboard", "Admin", new { area = "Corporate" }),
                "Staff" => RedirectToAction("Calendar", "StaffExpenses", new { area = "Corporate" }),
                _ => RedirectToAction(nameof(Login))
            };
        }

        /// <summary>Nhánh 1: sau đăng nhập/đăng ký User vào CRUD chi tiêu cá nhân.</summary>
        private IActionResult RedirectToPersonalExpenses() =>
            RedirectToAction("Calendar", "Expenses", new { area = "Personal" });
    }
}
