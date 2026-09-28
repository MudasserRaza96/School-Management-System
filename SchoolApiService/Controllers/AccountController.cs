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
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(ApiResponse<IEnumerable<UserDto>>.SuccessResponse(users, "Users retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UserDto>>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(ApiResponse<UserDto>.ErrorResponse($"User with Id '{id}' was not found.", statusCode: 404));
            }
            return Ok(ApiResponse<UserDto>.SuccessResponse(user, "User retrieved successfully."));
        }

        [HttpGet("user-roles")]
        public async Task<ActionResult<ApiResponse<IEnumerable<UserRoleAssignmentDto>>>> GetUserRoles()
        {
            var userRoles = await _userService.GetUserRolesAsync();
            return Ok(ApiResponse<IEnumerable<UserRoleAssignmentDto>>.SuccessResponse(userRoles, "User roles retrieved successfully."));
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<RegistrationRequest>>> Register([FromBody] RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<RegistrationRequest>.ErrorResponse("Registration failed.", validationErrors, 400));
            }

            var (succeeded, errors, registeredRequest) = await _userService.RegisterAsync(request);

            if (succeeded && registeredRequest != null)
            {
                return CreatedAtAction(nameof(Register), new { email = registeredRequest.Email }, ApiResponse<RegistrationRequest>.SuccessResponse(registeredRequest, "User registered successfully.", 201));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { "Registration failed." };
            return BadRequest(ApiResponse<RegistrationRequest>.ErrorResponse("User registration failed.", errList, 400));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUser(string id, [FromBody] UserDto request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed.", validationErrors, 400));
            }

            var (succeeded, errors, message) = await _userService.UpdateUserAsync(id, request);

            if (succeeded)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { id, username = request.Username, email = request.Email }, message ?? "User updated successfully."));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "User update failed." };
            return BadRequest(ApiResponse<object>.ErrorResponse("User update failed.", errList, 400));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteUser(string id)
        {
            var (succeeded, errors, message) = await _userService.DeleteUserAsync(id);

            if (succeeded)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { id }, message ?? "User deleted successfully."));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "User deletion failed." };
            return BadRequest(ApiResponse<object>.ErrorResponse("User deletion failed.", errList, 400));
        }

        [HttpPost("create-role")]
        public async Task<ActionResult<ApiResponse<UserRoleDto>>> CreateRole([FromBody] UserRoleDto request)
        {
            var (succeeded, errors, role) = await _userService.CreateRoleAsync(request);

            if (succeeded && role != null)
            {
                return Ok(ApiResponse<UserRoleDto>.SuccessResponse(role, "Role created successfully."));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { "Role creation failed." };
            return BadRequest(ApiResponse<UserRoleDto>.ErrorResponse("Role creation failed.", errList, 400));
        }

        [HttpPost("assign-role")]
        public async Task<ActionResult<ApiResponse<object>>> AssignRole([FromBody] AssignRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed.", validationErrors, 400));
            }

            var (succeeded, errors, message) = await _userService.AssignRoleAsync(request);

            if (succeeded)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { username = request.Username, role = request.Role, roles = request.Roles }, message ?? "Role assigned successfully."));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "Assign role failed." };
            return BadRequest(ApiResponse<object>.ErrorResponse("Role assignment failed.", errList, 400));
        }

        [HttpPut("update-user-role")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateUserRole([FromBody] AssignRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<object>.ErrorResponse("Validation failed.", validationErrors, 400));
            }

            var (succeeded, errors, message) = await _userService.UpdateUserRoleAsync(request);

            if (succeeded)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { username = request.Username, role = request.Role, roles = request.Roles }, message ?? "User role updated successfully."));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "User role update failed." };
            return BadRequest(ApiResponse<object>.ErrorResponse("User role update failed.", errList, 400));
        }

        [HttpDelete("remove-user-role/{username}/{roleName}")]
        public async Task<ActionResult<ApiResponse<object>>> RemoveUserRole(string username, string roleName)
        {
            var (succeeded, errors, message) = await _userService.RemoveUserRoleAsync(username, roleName);

            if (succeeded)
            {
                return Ok(ApiResponse<object>.SuccessResponse(new { username, roleName }, message ?? "Role removed successfully."));
            }

            var errList = errors?.Select(e => e.Description).ToList() ?? new List<string> { message ?? "Remove role failed." };
            return BadRequest(ApiResponse<object>.ErrorResponse("Role removal failed.", errList, 400));
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> Authenticate([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Authentication failed.", errors, 400));
            }

            var (succeeded, errorMessage, response) = await _userService.AuthenticateAsync(request);

            if (!succeeded)
            {
                if (errorMessage == "User not found in database")
                {
                    return Unauthorized(ApiResponse<AuthResponse>.ErrorResponse("User not found in database.", statusCode: 401));
                }
                return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(errorMessage ?? "Bad credentials.", statusCode: 400));
            }

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response!, "User authenticated successfully."));
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout()
        {
            return Ok(ApiResponse<object>.SuccessResponse(null!, "Logged out successfully."));
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponse>>> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponse<AuthResponse>.ErrorResponse("Invalid refresh token request.", errors, 400));
            }

            var (succeeded, errorMessage, response) = await _userService.RefreshTokenAsync(request);

            if (!succeeded)
            {
                return BadRequest(ApiResponse<AuthResponse>.ErrorResponse(errorMessage ?? "Invalid or expired refresh token.", statusCode: 400));
            }

            return Ok(ApiResponse<AuthResponse>.SuccessResponse(response!, "Token refreshed successfully."));
        }

        [HttpPost("revoke/{username}")]
        public async Task<ActionResult<ApiResponse<object>>> RevokeToken(string username)
        {
            var (succeeded, message) = await _userService.RevokeTokenAsync(username);
            if (!succeeded)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(message ?? "Revocation failed.", statusCode: 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, message ?? "Token revoked successfully."));
        }
    }
}