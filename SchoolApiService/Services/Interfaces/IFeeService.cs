using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IFeeService
    {
        Task<IEnumerable<FeeDto>> GetAllFeesAsync();
        Task<FeeDto?> GetFeeByIdAsync(int id);
        Task<FeeDto> CreateFeeAsync(FeeDto dto);
        Task<bool> UpdateFeeAsync(int id, FeeDto dto);
        Task<bool> DeleteFeeAsync(int id);
        Task<bool> FeeExistsAsync(int id);
    }
}
