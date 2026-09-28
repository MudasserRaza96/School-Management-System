using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels.SecurityModels;

namespace SchoolApiService.Controllers
{
    [Authorize]
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
        public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleDto>>>> GetRoles()
        {
            var roles = await _userService.GetAllRolesAsync();
            return Ok(ApiResponse<IEnumerable<UserRoleDto>>.SuccessResponse(roles, "Roles retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserRoleDto>>> GetRoleById(string id)
        {
            var role = await _userService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound(ApiResponse<UserRoleDto>.ErrorResponse(
                    "Role not found.",
                    new List<string> { $"Role with Id '{id}' was not found." },
                    404
                ));
            }

            return Ok(ApiResponse<UserRoleDto>.SuccessResponse(role, "Role retrieved successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UserRoleDto>>> CreateRole([FromBody] UserRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                var errList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<UserRoleDto>.ErrorResponse("Validation failed.", errList, 400));
            }

            var (succeeded, errors, createdRole) = await _userService.CreateRoleAsync(request);

            if (succeeded && createdRole != null)
            {
                return CreatedAtAction(nameof(GetRoleById), new { id = createdRole.Id },
                    ApiResponse<UserRoleDto>.SuccessResponse(createdRole, "Role created successfully.", 201));
            }

            var errorMessages = errors?.Select(e => e.Description).ToList() ?? new List<string> { "Failed to create role." };
            return BadRequest(ApiResponse<UserRoleDto>.ErrorResponse("Role creation failed.", errorMessages, 400));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UserRoleDto>>> UpdateRole(string id, [FromBody] UserRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                var errList = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<UserRoleDto>.ErrorResponse("Validation failed.", errList, 400));
            }

            var (succeeded, errors, message) = await _userService.UpdateRoleAsync(id, request);

            if (succeeded)
            {
                return Ok(ApiResponse<UserRoleDto>.SuccessResponse(request, message ?? "Role updated successfully.", 200));
            }

            var errorMessages = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "Role update failed." };
            return BadRequest(ApiResponse<UserRoleDto>.ErrorResponse("Role update failed.", errorMessages, 400));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteRole(string id)
        {
            var (succeeded, errors, message) = await _userService.DeleteRoleAsync(id);

            if (succeeded)
            {
                return Ok(ApiResponse<string>.SuccessResponse(id, message ?? "Role deleted successfully.", 200));
            }

            var errorMessages = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "Role deletion failed." };
            return BadRequest(ApiResponse<string>.ErrorResponse("Role deletion failed.", errorMessages, 400));
        }
    }
}
