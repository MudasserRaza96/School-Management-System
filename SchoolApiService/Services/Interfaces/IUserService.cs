using Microsoft.AspNetCore.Identity;
using SchoolApp.Models.DataModels.SecurityModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, RegistrationRequest? Request)> RegisterAsync(RegistrationRequest request);
        Task<List<UserRoleDto>> GetAllRolesAsync();
        Task<UserRoleDto?> GetRoleByIdAsync(string id);
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, UserRoleDto? Role)> CreateRoleAsync(UserRoleDto request);
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> UpdateRoleAsync(string id, UserRoleDto request);
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> DeleteRoleAsync(string id);
        Task<(bool Succeeded, string? ErrorMessage, AuthResponse? Response)> AuthenticateAsync(AuthRequest request);
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, string? Message)> AssignRoleAsync(AssignRoleDto request);
    }
}
