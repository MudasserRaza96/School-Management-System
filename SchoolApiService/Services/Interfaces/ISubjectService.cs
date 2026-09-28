using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<SubjectDto>> GetAllAsync();
        Task<SubjectDto?> GetByIdAsync(int id);
        Task<SubjectDto> CreateAsync(SubjectDto dto);
        Task<bool> UpdateAsync(int id, SubjectDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
