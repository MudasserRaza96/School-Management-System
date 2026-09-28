using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IFeeTypeService
    {
        Task<IEnumerable<FeeTypeDto>> GetAllAsync();
        Task<FeeTypeDto?> GetByIdAsync(int id);
        Task<FeeTypeDto> CreateAsync(FeeTypeDto dto);
        Task<bool> UpdateAsync(int id, FeeTypeDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
