using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public abstract class AdminBaseController : Controller
    {
        protected void SetError(string message) => TempData["Error"] = message;
        protected void SetSuccess(string message) => TempData["Success"] = message;
    }
}
