using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly SchoolDbContext _context;

        public AttendanceService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendancesAsync()
        {
            return await _context.dbsAttendance.ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(int id)
        {
            return await _context.dbsAttendance.FindAsync(id);
        }

        public async Task<IEnumerable<AttList>> GetAttendanceListByTypeAsync(AttendanceType type)
        {
            List<AttList> data = new List<AttList>();
            switch (type)
            {
                case AttendanceType.Student:
                    data = await _context.dbsStudent.Select(s => new AttList()
                    {
                        AttId = s.UniqueStudentAttendanceNumber,
                        Name = $"{s.StudentId} - " + s.StudentName
                    }).ToListAsync();
                    break;
                case AttendanceType.Staff:
                    data = await _context.dbsStaff.Select(s => new AttList()
                    {
                        AttId = s.UniqueStaffAttendanceNumber,
                        Name = $"{s.StaffId} - " + s.StaffName
                    }).ToListAsync();
                    break;
            }
            return data;
        }

        public async Task<(bool Succeeded, string? ErrorMessage, Attendance? CreatedAttendance)> CreateAttendanceAsync(Attendance attendance)
        {
            if (!Enum.IsDefined(typeof(AttendanceType), attendance.Type))
            {
                return (false, "Invalid attendance type.", null);
            }

            bool exists = false;
            switch (attendance.Type)
            {
                case AttendanceType.Student:
                    exists = await _context.dbsStudent.AnyAsync(s => s.UniqueStudentAttendanceNumber == attendance.AttendanceIdentificationNumber);
                    break;
                case AttendanceType.Staff:
                    exists = await _context.dbsStaff.AnyAsync(s => s.UniqueStaffAttendanceNumber == attendance.AttendanceIdentificationNumber);
                    break;
                default:
                    return (false, "Invalid attendance type.", null);
            }

            if (!exists)
            {
                return (false, "Invalid attendance identification number.", null);
            }

            _context.dbsAttendance.Add(attendance);
            await _context.SaveChangesAsync();
            return (true, null, attendance);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateAttendanceAsync(int id, Attendance attendance)
        {
            if (id != attendance.AttendanceId)
            {
                return (false, "Invalid AttendanceId", false);
            }

            if (attendance.Type == AttendanceType.Student)
            {
                var studentExists = await _context.dbsStudent.AnyAsync(s => s.UniqueStudentAttendanceNumber == attendance.AttendanceIdentificationNumber);
                if (!studentExists)
                {
                    return (false, "Invalid student attendance number", false);
                }
            }
            else if (attendance.Type == AttendanceType.Staff)
            {
                var staffExists = await _context.dbsStaff.AnyAsync(s => s.UniqueStaffAttendanceNumber == attendance.AttendanceIdentificationNumber);
                if (!staffExists)
                {
                    return (false, "Invalid staff attendance number", false);
                }
            }
            else
            {
                return (false, "Invalid attendance type", false);
            }

            _context.Entry(attendance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AttendanceExistsAsync(id))
                {
                    return (false, null, true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.dbsAttendance.FindAsync(id);
            if (attendance == null)
            {
                return false;
            }

            _context.dbsAttendance.Remove(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AttendanceExistsAsync(int id)
        {
            return await _context.dbsAttendance.AnyAsync(e => e.AttendanceId == id);
        }
    }
}
