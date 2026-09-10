using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IMarkService
    {
        Task<IEnumerable<Mark>> GetAllMarksAsync();
        Task<Mark?> GetMarkByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage, object? Result)> CreateMarkAsync(Mark mark);
        Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateMarkAsync(int id, Mark mark);
        Task<bool> DeleteMarkAsync(int id);
        Task<bool> MarkExistsAsync(int id);
    }
}
