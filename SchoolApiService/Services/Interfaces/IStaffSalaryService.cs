using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStaffSalaryService
    {
        Task<IEnumerable<StaffSalary>> GetAllStaffSalariesAsync();
        Task<StaffSalary?> GetStaffSalaryByIdAsync(int id);
        Task<StaffSalary> CreateStaffSalaryAsync(StaffSalary staffSalary);
        Task<(bool Succeeded, bool ConcurrencyError)> UpdateStaffSalaryAsync(int id, StaffSalary staffSalary);
        Task<bool> DeleteStaffSalaryAsync(int id);
        Task<bool> StaffSalaryExistsAsync(int id);
    }
}
