using SchoolApiService.ViewModels;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<Attendance>> GetAllAttendancesAsync();
        Task<Attendance?> GetAttendanceByIdAsync(int id);
        Task<IEnumerable<AttList>> GetAttendanceListByTypeAsync(AttendanceType type);
        Task<(bool Succeeded, string? ErrorMessage, Attendance? CreatedAttendance)> CreateAttendanceAsync(Attendance attendance);
        Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateAttendanceAsync(int id, Attendance attendance);
        Task<bool> DeleteAttendanceAsync(int id);
        Task<bool> AttendanceExistsAsync(int id);
    }
}
