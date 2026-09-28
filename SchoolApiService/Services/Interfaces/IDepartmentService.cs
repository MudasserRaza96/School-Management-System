using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IDepartmentService
    {
        Task<IEnumerable<DepartmentDto>> GetAllAsync();
        Task<DepartmentDto?> GetByIdAsync(int id);
        Task<DepartmentDto> CreateAsync(DepartmentDto dto);
        Task<bool> UpdateAsync(int id, DepartmentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
