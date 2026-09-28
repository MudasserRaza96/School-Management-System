using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffResponseDto>> GetAllStaffAsync();
        Task<StaffResponseDto?> GetStaffByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage, StaffResponseDto? CreatedStaff)> CreateStaffAsync(StaffCreateDto dto);
        Task<(bool Succeeded, string? ErrorMessage, StaffResponseDto? Staff)> UpdateStaffAsync(int id, StaffUpdateDto dto);
        Task<bool> DeleteStaffAsync(int id);
        Task<bool> StaffExistsAsync(int id);
    }
}
