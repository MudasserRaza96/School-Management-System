using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStaffExperienceService
    {
        Task<IEnumerable<StaffExperience>> GetAllStaffExperiencesAsync();
        Task<StaffExperience?> GetStaffExperienceByIdAsync(int id);
        Task<StaffExperience> CreateStaffExperienceAsync(StaffExperience staffExperience);
        Task<(bool Succeeded, bool ConcurrencyError)> UpdateStaffExperienceAsync(int id, StaffExperience staffExperience);
        Task<bool> DeleteStaffExperienceAsync(int id);
        Task<bool> StaffExperienceExistsAsync(int id);
    }
}
