using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IMarkService
    {
        Task<IEnumerable<MarkDto>> GetAllMarksAsync();
        Task<MarkDto?> GetMarkByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage, MarkDto? CreatedMark)> CreateMarkAsync(MarkDto dto);
        Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateMarkAsync(int id, MarkDto dto);
        Task<bool> DeleteMarkAsync(int id);
        Task<bool> MarkExistsAsync(int id);
    }
}
