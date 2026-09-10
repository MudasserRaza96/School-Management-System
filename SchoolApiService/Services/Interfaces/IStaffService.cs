using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<Staff>> GetAllStaffAsync();
        Task<Staff?> GetStaffByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage, Staff? CreatedStaff)> CreateStaffAsync(Staff staff);
        Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateStaffAsync(int id, Staff staff);
        Task<bool> DeleteStaffAsync(int id);
        Task<bool> StaffExistsAsync(int id);
    }
}
