using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return User.FindFirstValue(ClaimTypes.Role) switch
                {
                    "User" => RedirectToAction("Index", "Dashboard", new { area = "Personal" }),
                    "Admin" => RedirectToAction("Dashboard", "Admin", new { area = "Corporate" }),
                    "Staff" => RedirectToAction("Dashboard", "Staff", new { area = "Corporate" }),
                    _ => RedirectToAction("Login", "Auth")
                };
            }

            return RedirectToAction("Login", "Auth");
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() =>
            View(new Models.ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
