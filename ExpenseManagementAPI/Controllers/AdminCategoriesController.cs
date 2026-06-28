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
    [Route("api/admin/categories")]
    public class AdminCategoriesController : ControllerBase
    {
        private readonly IAdminCategoryService _service;

        public AdminCategoriesController(IAdminCategoryService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetForCompany(User.GetCompanyId()));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.CreateAsync(User.GetCompanyId(), request);
            return success ? Ok(data) : BadRequest(new { message = error });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdminCategoryRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.UpdateAsync(User.GetCompanyId(), id, request);
            return success ? Ok(data) : BadRequest(new { message = error });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, error) = await _service.DeleteAsync(User.GetCompanyId(), id);
            return success ? NoContent() : BadRequest(new { message = error });
        }
    }
}
