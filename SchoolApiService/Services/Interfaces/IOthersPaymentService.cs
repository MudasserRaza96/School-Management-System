using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IOthersPaymentService
    {
        Task<IEnumerable<OthersPaymentDto>> GetAllAsync();
        Task<OthersPaymentDto?> GetByIdAsync(int id);
        Task<OthersPaymentDto> CreateAsync(OthersPaymentDto dto);
        Task<OthersPaymentDto?> UpdateAsync(int id, OthersPaymentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
