using ExpenseManagementAPI.DTOs.Personal;
using ExpenseManagementAPI.Helpers;
using ExpenseManagementAPI.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace ExpenseManagementAPI.Controllers
{
    [Authorize(Roles = "User")]
    [ApiController]
    [Route("api/personal/wallets")]
    public class PersonalWalletsController : ControllerBase
    {
        private readonly IPersonalWalletService _service;

        public PersonalWalletsController(IPersonalWalletService service) => _service = service;

        [HttpGet]
        [EnableQuery(PageSize = 100)]
        public IActionResult Get() => Ok(_service.GetForUser(User.GetUserId()));

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var (success, data, error) = await _service.GetByIdAsync(User.GetUserId(), id);
            return success ? Ok(data) : NotFound(new { message = error });
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WalletRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.CreateAsync(User.GetUserId(), request);
            return success ? CreatedAtAction(nameof(GetById), new { id = data!.Id }, data) : BadRequest(new { message = error });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] WalletRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var (success, data, error) = await _service.UpdateAsync(User.GetUserId(), id, request);
            return success ? Ok(data) : BadRequest(new { message = error });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Deactivate(int id)
        {
            var (success, error) = await _service.DeactivateAsync(User.GetUserId(), id);
            return success ? NoContent() : BadRequest(new { message = error });
        }
    }
}
