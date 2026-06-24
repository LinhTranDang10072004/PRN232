using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Controllers
{
    public class HomeController : Controller
    {
        [AllowAnonymous]
        public IActionResult Index()
        {
            if (User.IsInRole("User"))
                return RedirectToAction("Index", "Dashboard", new { area = "Personal" });
            return View();
        }
    }
}
