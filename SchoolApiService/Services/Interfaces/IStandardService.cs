using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStandardService
    {
        Task<IEnumerable<Standard>> GetAllAsync();
        Task<Standard?> GetByIdAsync(int id);
        Task<Standard> CreateAsync(Standard standard);
        Task<bool> UpdateAsync(int id, Standard standard);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
