using SchoolApiService.DTOs;

namespace SchoolApiService.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync();
        Task<StudentResponseDto?> GetStudentByIdAsync(int id);
        Task<(bool Succeeded, string? ErrorMessage, StudentResponseDto? Student)> UpdateStudentAsync(int id, StudentUpdateDto dto);
        Task<(bool Succeeded, string? ErrorMessage, StudentResponseDto? CreatedStudent)> CreateStudentAsync(StudentCreateDto dto);
        Task<bool> DeleteStudentAsync(int id);
        Task<bool> StudentExistsAsync(int id);
    }
}
