using Microsoft.AspNetCore.Identity;
using SchoolApp.Models.DataModels.SecurityModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IUserService
    {
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors, RegistrationRequest? Request)> RegisterAsync(RegistrationRequest request);
        Task<(bool Succeeded, IEnumerable<IdentityError>? Errors)> CreateRoleAsync(UserRoleDto request);
        Task<(bool Succeeded, string? ErrorMessage, AuthResponse? Response)> AuthenticateAsync(AuthRequest request);
    }
}
