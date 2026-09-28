using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IAcademicMonthService
    {
        Task<IEnumerable<AcademicMonthDto>> GetAllAsync();
        Task<AcademicMonthDto?> GetByIdAsync(int id);
        Task<AcademicMonthDto> CreateAsync(AcademicMonthDto dto);
        Task<bool> UpdateAsync(int id, AcademicMonthDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
