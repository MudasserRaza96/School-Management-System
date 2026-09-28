using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStaffSalaryService
    {
        Task<IEnumerable<StaffSalaryDto>> GetAllStaffSalariesAsync();
        Task<StaffSalaryDto?> GetStaffSalaryByIdAsync(int id);
        Task<StaffSalaryDto> CreateStaffSalaryAsync(StaffSalaryDto dto);
        Task<bool> UpdateStaffSalaryAsync(int id, StaffSalaryDto dto);
        Task<bool> DeleteStaffSalaryAsync(int id);
        Task<bool> StaffSalaryExistsAsync(int id);
    }
}
