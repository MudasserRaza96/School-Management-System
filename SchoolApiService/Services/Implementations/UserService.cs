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

        public async Task<List<UserRoleDto>> GetAllRolesAsync()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return roles.Select(r => new UserRoleDto
            {
                Id = r.Id,
                Name = r.Name ?? string.Empty
            }).ToList();
        }

        public async Task<UserRoleDto?> GetRoleByIdAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return null;

            return new UserRoleDto
            {
                Id = role.Id,
                Name = role.Name ?? string.Empty
            };
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, UserRoleDto? Role)> CreateRoleAsync(UserRoleDto request)
        {
            var role = new IdentityRole { Name = request.Name };
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                request.Id = role.Id;
                return (true, null, request);
            }

            return (false, result.Errors, null);
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> UpdateRoleAsync(string id, UserRoleDto request)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                var identityError = new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"Role with Id '{id}' was not found."
                };
                return (false, new[] { identityError }, $"Role with Id '{id}' not found.");
            }

            role.Name = request.Name;
            var result = await _roleManager.UpdateAsync(role);

            if (result.Succeeded)
            {
                return (true, null, $"Role '{request.Name}' updated successfully.");
            }

            return (false, result.Errors, "Failed to update role.");
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> DeleteRoleAsync(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                var identityError = new IdentityError
                {
                    Code = "RoleNotFound",
                    Description = $"Role with Id '{id}' was not found."
                };
                return (false, new[] { identityError }, $"Role with Id '{id}' not found.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                return (true, null, $"Role '{role.Name}' deleted successfully.");
            }

            return (false, result.Errors, "Failed to delete role.");
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

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> AssignRoleAsync(AssignRoleDto request)
        {
            var user = await _userManager.FindByNameAsync(request.Username) 
                       ?? await _userManager.FindByEmailAsync(request.Username);

            if (user == null)
            {
                var identityError = new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"User '{request.Username}' was not found."
                };
                return (false, new[] { identityError }, $"User '{request.Username}' not found.");
            }

            var rolesToAssign = new List<string>();
            if (!string.IsNullOrWhiteSpace(request.Role))
            {
                rolesToAssign.Add(request.Role);
            }

            if (request.Roles != null && request.Roles.Count > 0)
            {
                foreach (var r in request.Roles)
                {
                    if (!string.IsNullOrWhiteSpace(r) && !rolesToAssign.Contains(r))
                    {
                        rolesToAssign.Add(r);
                    }
                }
            }

            if (rolesToAssign.Count == 0)
            {
                var identityError = new IdentityError
                {
                    Code = "NoRolesProvided",
                    Description = "No role specified to assign."
                };
                return (false, new[] { identityError }, "No role specified.");
            }

            foreach (var roleName in rolesToAssign)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
            }

            var result = await _userManager.AddToRolesAsync(user, rolesToAssign);
            if (!result.Succeeded)
            {
                return (false, result.Errors, "Failed to assign role(s).");
            }

            user.Role ??= new List<string>();
            foreach (var roleName in rolesToAssign)
            {
                if (!user.Role.Contains(roleName))
                {
                    user.Role.Add(roleName);
                }
            }
            await _userManager.UpdateAsync(user);

            return (true, null, $"Role(s) [{string.Join(", ", rolesToAssign)}] assigned successfully to user '{user.UserName}'.");
        }
    }
}
