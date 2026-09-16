using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels.SecurityModels;

namespace SchoolApiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IUserService userService, ILogger<RolesController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserRoleDto>> GetRoleById(string id)
        {
            var role = await _userService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { message = $"Role with Id '{id}' was not found." });
            }

            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<UserRoleDto>> CreateRole([FromBody] UserRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, createdRole) = await _userService.CreateRoleAsync(request);

            if (succeeded && createdRole != null)
            {
                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id }, createdRole);
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(string id, [FromBody] UserRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, message) = await _userService.UpdateRoleAsync(id, request);

            if (succeeded)
            {
                return Ok(new { message, id, name = request.Name });
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var (succeeded, errors, message) = await _userService.DeleteRoleAsync(id);

            if (succeeded)
            {
                return Ok(new { message, id });
            }

            if (errors != null)
            {
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
            }

            return BadRequest(ModelState);
        }
    }
}
