using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<Student>> GetAllStudentsAsync();
        Task<Student?> GetStudentByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateStudentAsync(int id, Student student);
        Task<(bool Succeeded, string? ErrorMessage, Student? CreatedStudent)> CreateStudentAsync(Student student);
        Task<bool> DeleteStudentAsync(int id);
        Task<bool> StudentExistsAsync(int id);
    }
}
