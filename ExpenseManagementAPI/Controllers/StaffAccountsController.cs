using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "Staff")]
    [ApiController]
    [Route("api/staff/accounts")]
    public class StaffAccountsController : ControllerBase
    {
        private readonly IStaffAccountService _service;

        public StaffAccountsController(IStaffAccountService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetForCompany(User.GetCompanyId()));
    }
}
