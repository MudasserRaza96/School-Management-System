using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels.SecurityModels;

namespace SchoolApiService.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SchoolDbContext _context;
        private readonly ITokenService _tokenService;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            SchoolDbContext context,
            ITokenService tokenService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _tokenService = tokenService;
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, RegistrationRequest? Request)> RegisterAsync(RegistrationRequest request)
        {
            var user = new ApplicationUser 
            { 
                UserName = request.Username, 
                Email = request.Email, 
                Role = request.Role 
            };

            var result = await _userManager.CreateAsync(user, request.Password!);

            if (result.Succeeded)
            {
                request.Password = "";
                return (true, null, request);
            }

            return (false, result.Errors, null);
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors)> CreateRoleAsync(UserRoleDto request)
        {
            var role = new IdentityRole { Name = request.Name };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                return (true, null);
            }

            return (false, result.Errors);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, AuthResponse? Response)> AuthenticateAsync(AuthRequest request)
        {
            var managedUser = await _userManager.FindByEmailAsync(request.Email!);
            if (managedUser == null)
            {
                return (false, "Bad credentials", null);
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(managedUser, request.Password!);
            if (!isPasswordValid)
            {
                return (false, "Bad credentials", null);
            }

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (userInDb == null)
            {
                return (false, "User not found in database", null);
            }

            var accessToken = _tokenService.CreateToken(userInDb);
            await _context.SaveChangesAsync();

            var response = new AuthResponse
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken
            };

            return (true, null, response);
        }
    }
}
