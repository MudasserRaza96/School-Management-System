using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IExamTypeService
    {
        Task<IEnumerable<ExamType>> GetAllExamTypesAsync();
        Task<ExamType?> GetExamTypeByIdAsync(int id);
        Task<ExamType> CreateExamTypeAsync(ExamType examType);
        Task<(bool Succeeded, bool ConcurrencyError)> UpdateExamTypeAsync(int id, ExamType examType);
        Task<bool> DeleteExamTypeAsync(int id);
        Task<bool> ExamTypeExistsAsync(int id);
    }
}
