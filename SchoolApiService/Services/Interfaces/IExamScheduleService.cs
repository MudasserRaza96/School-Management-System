using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;
using static SchoolApiService.Controllers.ExamSchedulesController;

namespace SchoolApiService.Services.Interfaces
{
    public interface IExamScheduleService
    {
        Task<IEnumerable<ExamScheduleVM>> GetAllExamSchedulesAsync();
        Task<IEnumerable<GetExamScheduleOptionsResponse>> GetExamScheduleOptionsAsync();
        Task<ExamScheduleVM?> GetExamScheduleByIdAsync(int id);
        Task<ExamSchedule> CreateExamScheduleAsync(ExamSchedule examSchedule);
        Task<(bool Succeeded, bool ConcurrencyError)> UpdateExamScheduleAsync(int id, ExamSchedule examSchedule);
        Task<bool> DeleteExamScheduleAsync(int id);
        Task<bool> ExamScheduleExistsAsync(int id);
    }
}
