using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IMonthlyPaymentService
    {
        Task<IEnumerable<MonthlyPaymentDto>> GetAllAsync();
        Task<MonthlyPaymentDto?> GetByIdAsync(int id);
        Task<MonthlyPaymentDto> CreateAsync(MonthlyPaymentDto dto);
        Task<MonthlyPaymentDto?> UpdateAsync(int id, MonthlyPaymentDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
