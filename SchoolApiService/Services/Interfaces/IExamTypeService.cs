using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IExamTypeService
    {
        Task<IEnumerable<ExamTypeDto>> GetAllAsync();
        Task<ExamTypeDto?> GetByIdAsync(int id);
        Task<ExamTypeDto> CreateAsync(ExamTypeDto dto);
        Task<bool> UpdateAsync(int id, ExamTypeDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
