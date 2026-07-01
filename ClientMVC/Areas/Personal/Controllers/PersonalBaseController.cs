using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Personal.Controllers
{
    [Area("Personal")]
    [Authorize(Roles = "User")]
    public abstract class PersonalBaseController : Controller
    {
        protected void SetError(string message) => TempData["Error"] = message;
        protected void SetSuccess(string message) => TempData["Success"] = message;
        protected void SetWarning(string message) => TempData["Warning"] = message;
    }
}
