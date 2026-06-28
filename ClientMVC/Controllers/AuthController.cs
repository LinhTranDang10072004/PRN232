using System.Security.Claims;
using ClientMVC.Models;
using ClientMVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthApiClient _authApi;

        public AuthController(IAuthApiClient authApi)
        {
            _authApi = authApi;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
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

            var (success, data, error) = await _authApi.LoginAsync(model);
            if (!success || data == null)
            {
                model.ErrorMessage = error ?? "Đăng nhập thất bại.";
                return View(model);
            }

            await SignInUserAsync(data);
            if (data.Role == "User")
                return RedirectToAction("Index", "Dashboard", new { area = "Personal" });
            if (data.Role == "Staff")
                return RedirectToAction("Index", "Dashboard", new { area = "Staff" });
            if (data.Role == "Admin")
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            return RedirectToLocal(returnUrl);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var (success, data, error) = await _authApi.RegisterAsync(model);
            if (!success || data == null)
            {
                model.ErrorMessage = error ?? "Đăng ký thất bại.";
                return View(model);
            }

            await SignInUserAsync(data);
            if (data.Role == "User")
                return RedirectToAction("Index", "Dashboard", new { area = "Personal" });
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        private async Task SignInUserAsync(AuthResult auth)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, auth.UserId.ToString()),
                new(ClaimTypes.Name, auth.Username),
                new(ClaimTypes.Role, auth.Role),
                new("access_token", auth.Token)
            };

            if (!string.IsNullOrWhiteSpace(auth.FullName))
                claims.Add(new Claim("FullName", auth.FullName));

            if (auth.CompanyId.HasValue)
                claims.Add(new Claim("companyId", auth.CompanyId.Value.ToString()));

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

        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            return RedirectToAction("Index", "Home");
        }
    }
}
