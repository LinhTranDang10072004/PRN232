using System.Security.Claims;
using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    /// <summary>
    /// Nhánh 1: CRUD danh mục Personal — xem hệ thống + tự tạo/sửa/xóa danh mục của mình.
    /// </summary>
    [ApiController]
    [Route("api/personal/categories")]
    [Authorize(Roles = "User")]
    public class PersonalCategoriesController : ControllerBase
    {
        private readonly IPersonalCategoryService _categoryService;

        public PersonalCategoriesController(IPersonalCategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// OData: $filter, $orderby, $top, $skip, $select, $count.
        /// Ví dụ: ?$filter=contains(Name,'ăn')&$orderby=Name
        /// </summary>
        [HttpGet]
        [EnableQuery(PageSize = 50)]
        public IActionResult GetAll()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            return Ok(_categoryService.GetMyCategories(userId.Value));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> GetById(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var category = await _categoryService.GetByIdAsync(userId.Value, id);
            if (category == null)
                return NotFound(new { message = "Không tìm thấy danh mục." });

            return Ok(category);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryResponse>> Create([FromBody] CreateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var (result, error) = await _categoryService.CreateAsync(userId.Value, request);
            if (error != null)
                return BadRequest(new { message = error });

            return CreatedAtAction(nameof(GetById), new { id = result!.Id }, result);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoryResponse>> Update(int id, [FromBody] UpdateCategoryRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var (result, error) = await _categoryService.UpdateAsync(userId.Value, id, request);
            if (error != null)
            {
                if (error.Contains("Không tìm thấy"))
                    return NotFound(new { message = error });
                return BadRequest(new { message = error });
            }

            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Unauthorized();

            var (success, error) = await _categoryService.DeleteAsync(userId.Value, id);
            if (!success)
            {
                if (error != null && error.Contains("Không tìm thấy"))
                    return NotFound(new { message = error });
                return BadRequest(new { message = error });
            }

            return NoContent();
        }

        private int? GetCurrentUserId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var id) ? id : null;
        }
    }
}
