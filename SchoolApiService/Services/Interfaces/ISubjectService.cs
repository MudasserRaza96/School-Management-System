using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface ISubjectService
    {
        Task<IEnumerable<Subject>> GetAllAsync();
        Task<Subject?> GetByIdAsync(int id);
        Task<Subject> CreateAsync(Subject subject);
        Task<bool> UpdateAsync(int id, Subject subject);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
