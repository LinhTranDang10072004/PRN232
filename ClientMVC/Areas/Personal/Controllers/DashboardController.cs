using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    [Area("Personal")]
    [Authorize(Roles = "User")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            ViewBag.Username = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.FullName = User.FindFirst("FullName")?.Value;
            ViewBag.Email = User.FindFirstValue(ClaimTypes.Email);
            return View();
        }
    }
}
