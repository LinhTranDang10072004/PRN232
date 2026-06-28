using ExpenseManagementAPI.DTOs.Admin;
using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "Admin")]
    [ApiController]
    [Route("api/admin/accounts")]
    public class AdminAccountsController : ControllerBase
    {
        private readonly IAdminAccountService _service;

        public AdminAccountsController(IAdminAccountService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetForCompany(User.GetCompanyId()));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.CreateAsync(User.GetCompanyId(), request);
            return success ? Ok(data) : BadRequest(new { message = error });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminAccountRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.UpdateAsync(User.GetCompanyId(), id, request);
            return success ? Ok(data) : BadRequest(new { message = error });
        }
    }
}
