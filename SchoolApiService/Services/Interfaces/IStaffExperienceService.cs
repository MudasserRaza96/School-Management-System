using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStaffExperienceService
    {
        Task<IEnumerable<StaffExperienceDto>> GetAllStaffExperiencesAsync();
        Task<StaffExperienceDto?> GetStaffExperienceByIdAsync(int id);
        Task<StaffExperienceDto> CreateStaffExperienceAsync(StaffExperienceDto dto);
        Task<bool> UpdateStaffExperienceAsync(int id, StaffExperienceDto dto);
        Task<bool> DeleteStaffExperienceAsync(int id);
        Task<bool> StaffExperienceExistsAsync(int id);
    }
}
