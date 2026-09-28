using SchoolApiService.DTOs;
using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync();
        Task<AttendanceDto?> GetAttendanceByIdAsync(int id);
        Task<IEnumerable<AttList>> GetAttendanceListByTypeAsync(AttendanceType type);
        Task<(bool Succeeded, string? ErrorMessage, AttendanceDto? CreatedAttendance)> CreateAttendanceAsync(AttendanceDto dto);
        Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateAttendanceAsync(int id, AttendanceDto dto);
        Task<bool> DeleteAttendanceAsync(int id);
        Task<bool> AttendanceExistsAsync(int id);
    }
}
