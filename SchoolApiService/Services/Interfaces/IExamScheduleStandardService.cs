using SchoolApiService.ViewModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IExamScheduleStandardService
    {
        Task<IEnumerable<ExamScheduleStandardVM>> GetAllExamScheduleStandardsAsync();
        Task<ExamScheduleStandardVM?> GetExamScheduleStandardByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage)> CreateExamScheduleStandardAsync(CreateExamScheduleStandardVM request);
        Task<(bool Succeeded, string? ErrorMessage)> UpdateExamScheduleStandardAsync(int id, UpdateExamScheduleStandardVM request);
        Task<bool> DeleteExamScheduleStandardAsync(int id);
        Task<bool> ExamScheduleStandardExistsAsync(int id);
    }
}
