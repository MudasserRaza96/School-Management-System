using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IDueBalanceService
    {
        Task<IEnumerable<DueBalance>> GetAllAsync();
        Task<DueBalance?> GetByIdAsync(int id);
        Task<DueBalance> CreateAsync(DueBalance dueBalance);
        Task<bool> UpdateAsync(int id, DueBalance dueBalance);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
