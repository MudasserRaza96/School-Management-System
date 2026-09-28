using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IDueBalanceService
    {
        Task<IEnumerable<DueBalanceDto>> GetAllAsync();
        Task<DueBalanceDto?> GetByIdAsync(int id);
        Task<DueBalanceDto> CreateAsync(DueBalanceDto dto);
        Task<bool> UpdateAsync(int id, DueBalanceDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
