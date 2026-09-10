using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IFeeService
    {
        Task<IEnumerable<Fee>> GetAllAsync();
        Task<Fee?> GetByIdAsync(int id);
        Task<Fee> CreateAsync(Fee fee);
        Task<bool> UpdateAsync(int id, Fee fee);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
