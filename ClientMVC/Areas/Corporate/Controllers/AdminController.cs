using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Corporate.Controllers
{
    [Area("Corporate")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            ViewBag.Username = User.FindFirstValue(ClaimTypes.Name);
            ViewBag.FullName = User.FindFirst("FullName")?.Value;
            ViewBag.Email = User.FindFirstValue(ClaimTypes.Email);
            return View();
        }
    }
}
