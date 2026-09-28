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

        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserDto>();

            foreach (var u in users)
            {
                var identityRoles = await _userManager.GetRolesAsync(u);
                var combinedRoles = identityRoles.Union(u.Role ?? new List<string>()).Distinct().ToList();

                result.Add(new UserDto
                {
                    Id = u.Id,
                    Username = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Role = combinedRoles
                });
            }

            return result;
        }

        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return null;

            var identityRoles = await _userManager.GetRolesAsync(user);
            var combinedRoles = identityRoles.Union(user.Role ?? new List<string>()).Distinct().ToList();

            return new UserDto
            {
                Id = user.Id,
                Username = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Role = combinedRoles
            };
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
                if (request.Role != null && request.Role.Count > 0)
                {
                    foreach (var roleName in request.Role)
                    {
                        var roleExists = await _roleManager.RoleExistsAsync(roleName);
                        if (!roleExists)
                        {
                            await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                        }
                    }
                    await _userManager.AddToRolesAsync(user, request.Role);
                }

                request.Password = "";
                return (true, null, request);
            }

            return (false, result.Errors, null);
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> UpdateUserAsync(string id, UserDto request)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                var error = new IdentityError { Code = "UserNotFound", Description = $"User with Id '{id}' not found." };
                return (false, new[] { error }, "User not found.");
            }

            user.UserName = request.Username;
            user.Email = request.Email;
            if (request.Role != null)
            {
                user.Role = request.Role;
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return (false, result.Errors, "Failed to update user.");
            }

            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                await _userManager.ResetPasswordAsync(user, token, request.Password);
            }

            return (true, null, $"User '{user.UserName}' updated successfully.");
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                var error = new IdentityError { Code = "UserNotFound", Description = $"User with Id '{id}' not found." };
                return (false, new[] { error }, "User not found.");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                return (true, null, $"User '{user.UserName}' deleted successfully.");
            }

            return (false, result.Errors, "Failed to delete user.");
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
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

            userInDb.RefreshToken = refreshToken;
            userInDb.RefreshTokenExpiryTime = refreshTokenExpiryTime;
            await _userManager.UpdateAsync(userInDb);

            var roles = await _userManager.GetRolesAsync(userInDb);

            var response = new AuthResponse
            {
                Username = userInDb.UserName,
                Email = userInDb.Email,
                Token = accessToken,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = refreshTokenExpiryTime,
                Roles = string.Join(",", roles)
            };

            return (true, null, response);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, AuthResponse? Response)> RefreshTokenAsync(RefreshTokenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return (false, "Invalid client request.", null);
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
            ApplicationUser? user = null;

            if (principal != null)
            {
                var username = principal.Identity?.Name 
                    ?? principal.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value
                    ?? principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (!string.IsNullOrEmpty(username))
                {
                    user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
                }
            }

            if (user == null)
            {
                user = await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == request.RefreshToken);
            }

            if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                return (false, "Invalid or expired refresh token.", null);
            }

            var newAccessToken = _tokenService.CreateToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newExpiryTime = DateTime.UtcNow.AddDays(7);

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = newExpiryTime;
            await _userManager.UpdateAsync(user);

            var roles = await _userManager.GetRolesAsync(user);

            var response = new AuthResponse
            {
                Username = user.UserName,
                Email = user.Email,
                Token = newAccessToken,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiryTime = newExpiryTime,
                Roles = string.Join(",", roles)
            };

            return (true, null, response);
        }

        public async Task<(bool Succeeded, string? Message)> RevokeTokenAsync(string username)
        {
            var user = await _userManager.FindByNameAsync(username) ?? await _userManager.FindByEmailAsync(username);
            if (user == null)
            {
                return (false, "User not found.");
            }

            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = null;
            await _userManager.UpdateAsync(user);

            return (true, "Token revoked successfully.");
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

        public async Task<List<UserRoleAssignmentDto>> GetUserRolesAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            var result = new List<UserRoleAssignmentDto>();

            foreach (var user in users)
            {
                var identityRoles = await _userManager.GetRolesAsync(user);
                var customRoles = user.Role ?? new List<string>();
                var combinedRoles = identityRoles.Union(customRoles).Distinct().ToList();

                result.Add(new UserRoleAssignmentDto
                {
                    UserId = user.Id,
                    Username = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    Roles = combinedRoles
                });
            }

            return result;
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> UpdateUserRoleAsync(AssignRoleDto request)
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

            foreach (var roleName in rolesToAssign)
            {
                var roleExists = await _roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = roleName });
                }
            }

            var currentIdentityRoles = await _userManager.GetRolesAsync(user);
            if (currentIdentityRoles.Count > 0)
            {
                await _userManager.RemoveFromRolesAsync(user, currentIdentityRoles);
            }

            if (rolesToAssign.Count > 0)
            {
                var result = await _userManager.AddToRolesAsync(user, rolesToAssign);
                if (!result.Succeeded)
                {
                    return (false, result.Errors, "Failed to update user role(s).");
                }
            }

            user.Role = rolesToAssign;
            await _userManager.UpdateAsync(user);

            return (true, null, $"Roles updated successfully for user '{user.UserName}'.");
        }

        public async Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> RemoveUserRoleAsync(string username, string roleName)
        {
            var user = await _userManager.FindByNameAsync(username)
                       ?? await _userManager.FindByEmailAsync(username);

            if (user == null)
            {
                var identityError = new IdentityError
                {
                    Code = "UserNotFound",
                    Description = $"User '{username}' was not found."
                };
                return (false, new[] { identityError }, $"User '{username}' not found.");
            }

            if (await _userManager.IsInRoleAsync(user, roleName))
            {
                var result = await _userManager.RemoveFromRoleAsync(user, roleName);
                if (!result.Succeeded)
                {
                    return (false, result.Errors, $"Failed to remove role '{roleName}'.");
                }
            }

            if (user.Role != null && user.Role.Contains(roleName))
            {
                user.Role.Remove(roleName);
                await _userManager.UpdateAsync(user);
            }

            return (true, null, $"Role '{roleName}' removed successfully from user '{user.UserName}'.");
        }
    }
}
