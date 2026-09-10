using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels.SecurityModels;

namespace SchoolApiService.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

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

        [HttpPost("create-role")]
        public async Task<ActionResult> CreateRole([FromBody] UserRoleDto request)
        {
            var (succeeded, errors) = await _userService.CreateRoleAsync(request);

            if (succeeded)
            {
                return Ok(request);
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