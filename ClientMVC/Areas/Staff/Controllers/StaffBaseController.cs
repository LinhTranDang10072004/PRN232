using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientMVC.Areas.Staff.Controllers
{
    [Area("Staff")]
    [Authorize(Roles = "Staff")]
    public abstract class StaffBaseController : Controller
    {
        protected void SetError(string message) => TempData["Error"] = message;
        protected void SetSuccess(string message) => TempData["Success"] = message;
    }
}
