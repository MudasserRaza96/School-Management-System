using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;

namespace SchoolApiService.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly SchoolDbContext _context;

        public AttendanceService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceDto>> GetAllAttendancesAsync()
        {
            var records = await _context.dbsAttendance.ToListAsync();
            return records.Select(r => new AttendanceDto
            {
                AttendanceId = r.AttendanceId,
                AttendanceDate = r.Date,
                StudentId = r.AttendanceIdentificationNumber,
                IsPresent = r.IsPresent,
                Remarks = r.Description
            });
        }

        public async Task<AttendanceDto?> GetAttendanceByIdAsync(int id)
        {
            var r = await _context.dbsAttendance.FindAsync(id);
            if (r == null) return null;

            return new AttendanceDto
            {
                AttendanceId = r.AttendanceId,
                AttendanceDate = r.Date,
                StudentId = r.AttendanceIdentificationNumber,
                IsPresent = r.IsPresent,
                Remarks = r.Description
            };
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

        public async Task<(bool Succeeded, string? ErrorMessage, AttendanceDto? CreatedAttendance)> CreateAttendanceAsync(AttendanceDto dto)
        {
            var entity = new Attendance
            {
                Date = dto.AttendanceDate,
                AttendanceIdentificationNumber = dto.StudentId,
                IsPresent = dto.IsPresent,
                Description = dto.Remarks,
                Type = AttendanceType.Student
            };

            _context.dbsAttendance.Add(entity);
            await _context.SaveChangesAsync();
            dto.AttendanceId = entity.AttendanceId;
            return (true, null, dto);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateAttendanceAsync(int id, AttendanceDto dto)
        {
            if (id != dto.AttendanceId)
            {
                return (false, "Invalid AttendanceId", false);
            }

            var entity = await _context.dbsAttendance.FindAsync(id);
            if (entity == null)
            {
                return (false, "Attendance record not found.", false);
            }

            entity.Date = dto.AttendanceDate;
            entity.AttendanceIdentificationNumber = dto.StudentId;
            entity.IsPresent = dto.IsPresent;
            entity.Description = dto.Remarks;

            await _context.SaveChangesAsync();
            return (true, null, false);
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            var attendance = await _context.dbsAttendance.FindAsync(id);
            if (attendance == null) return false;

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
