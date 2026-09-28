using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetUserById(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = $"User with Id '{id}' was not found." });
            }
            return Ok(user);
        }

        [HttpGet("user-roles")]
        public async Task<ActionResult<IEnumerable<UserRoleAssignmentDto>>> GetUserRoles()
        {
            var userRoles = await _userService.GetUserRolesAsync();
            return Ok(userRoles);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, registeredRequest) = await _userService.RegisterAsync(request);

            if (succeeded && registeredRequest != null)
            {
                return CreatedAtAction(nameof(Register), new { email = registeredRequest.Email, role = registeredRequest.Role }, registeredRequest);
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
        public async Task<IActionResult> UpdateUser(string id, [FromBody] UserDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, message) = await _userService.UpdateUserAsync(id, request);

            if (succeeded)
            {
                return Ok(new { message, id, username = request.Username, email = request.Email });
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
        public async Task<IActionResult> DeleteUser(string id)
        {
            var (succeeded, errors, message) = await _userService.DeleteUserAsync(id);

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

        [HttpPost("create-role")]
        public async Task<ActionResult> CreateRole([FromBody] UserRoleDto request)
        {
            var (succeeded, errors, role) = await _userService.CreateRoleAsync(request);

            if (succeeded)
            {
                return Ok(role ?? request);
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

        [HttpPost("assign-role")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, message) = await _userService.AssignRoleAsync(request);

            if (succeeded)
            {
                return Ok(new { message, username = request.Username, role = request.Role, roles = request.Roles });
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

        [HttpPut("update-user-role")]
        public async Task<IActionResult> UpdateUserRole([FromBody] AssignRoleDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errors, message) = await _userService.UpdateUserRoleAsync(request);

            if (succeeded)
            {
                return Ok(new { message, username = request.Username, role = request.Role, roles = request.Roles });
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

        [HttpDelete("remove-user-role/{username}/{roleName}")]
        public async Task<IActionResult> RemoveUserRole(string username, string roleName)
        {
            var (succeeded, errors, message) = await _userService.RemoveUserRoleAsync(username, roleName);

            if (succeeded)
            {
                return Ok(new { message, username, roleName });
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

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Authenticate([FromBody] AuthRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (succeeded, errorMessage, response) = await _userService.AuthenticateAsync(request);

            if (!succeeded)
            {
                if (errorMessage == "User not found in database")
                {
                    return Unauthorized(request);
                }
                return BadRequest(errorMessage ?? "Bad credentials");
            }

            return Ok(response);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {
            return Ok();
        }
    }
}