using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "Staff")]
    [ApiController]
    [Route("api/staff/categories")]
    public class StaffCategoriesController : ControllerBase
    {
        private readonly IStaffCategoryService _service;

        public StaffCategoriesController(IStaffCategoryService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetForCompany(User.GetCompanyId()));
    }
}
