using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStandardService
    {
        Task<IEnumerable<StandardDto>> GetAllAsync();
        Task<StandardDto?> GetByIdAsync(int id);
        Task<StandardDto> CreateAsync(StandardDto dto);
        Task<bool> UpdateAsync(int id, StandardDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
