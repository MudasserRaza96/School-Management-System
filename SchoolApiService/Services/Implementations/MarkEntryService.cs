using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class MarkEntryService : IMarkEntryService
    {
        private readonly SchoolDbContext _context;

        public MarkEntryService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MarkEntry>> GetAllMarkEntriesAsync()
        {
            return await _context.dbsMarkEntry
                .Include(m => m.Staff)
                .Include(m => m.ExamSchedule)
                .Include(m => m.ExamType)
                .Include(m => m.Subject)
                .Include(m => m.Standard)
                .Include(m => m.StudentMarksDetails)
                .ThenInclude(m => m.Student)
                .ToListAsync();
        }

        public async Task<MarkEntry?> GetMarkEntryByIdAsync(int id)
        {
            return await _context.dbsMarkEntry
                .Include(m => m.Staff)
                .Include(m => m.ExamSchedule)
                .Include(m => m.ExamType)
                .Include(m => m.Subject)
                .Include(m => m.Standard)
                .Include(m => m.StudentMarksDetails)
                .ThenInclude(m => m.Student)
                .FirstOrDefaultAsync(m => m.MarkEntryId == id);
        }

        public async Task<IEnumerable<StudentMarksDetails>> PopulateStudentMarksDetailsAsync(MarkEntry markEntry)
        {
            var students = await _context.dbsStudent
                .Where(s => s.StandardId == markEntry.StandardId)
                .ToListAsync();

            foreach (var student in students)
            {
                markEntry.StudentMarksDetails.Add(new StudentMarksDetails()
                {
                    StudentId = student.StudentId,
                    StudentName = student.StudentName,
                });
            }

            return markEntry.StudentMarksDetails;
        }

        public async Task<(bool Succeeded, string? ErrorMessage, int StatusCode, MarkEntry? CreatedEntry)> CreateMarkEntryAsync(MarkEntry markEntry)
        {
            try
            {
                if (markEntry.SubjectId == 0)
                {
                    return (false, "Please select a Subject and Students for marks entry.", 400, null);
                }

                await _context.dbsMarkEntry.AddAsync(markEntry);
                await _context.SaveChangesAsync();

                return (true, null, 201, markEntry);
            }
            catch (Exception ex)
            {
                return (false, $"Internal Server Error: {ex.Message}", 500, null);
            }
        }

        public async Task<(bool Succeeded, string? ErrorMessage, MarkEntry? UpdatedEntry)> UpdateMarkEntryAsync(MarkEntry markEntry)
        {
            if (markEntry.SubjectId == 0)
            {
                return (false, "Please select a Subject and Students for marks update.", null);
            }

            var existingMarkEntry = await _context.dbsMarkEntry
                .Include(m => m.Subject)
                .ThenInclude(s => s.Standard)
                .ThenInclude(std => std.Students)
                .Where(m => m.MarkEntryId == markEntry.MarkEntryId)
                .FirstOrDefaultAsync();

            if (existingMarkEntry == null)
            {
                return (false, "Mark entry not found.", null);
            }

            existingMarkEntry.StaffId = markEntry.StaffId;
            existingMarkEntry.ExamScheduleId = markEntry.ExamScheduleId;
            existingMarkEntry.ExamTypeId = markEntry.ExamTypeId;
            existingMarkEntry.TotalMarks = markEntry.TotalMarks;
            existingMarkEntry.PassMarks = markEntry.PassMarks;

            await _context.SaveChangesAsync();
            return (true, null, existingMarkEntry);
        }

        public async Task<bool> DeleteMarkEntryAsync(int id)
        {
            var markEntry = await _context.dbsMarkEntry.FindAsync(id);
            if (markEntry == null)
            {
                return false;
            }

            _context.dbsMarkEntry.Remove(markEntry);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkEntryExistsAsync(int id)
        {
            return await _context.dbsMarkEntry.AnyAsync(e => e.MarkEntryId == id);
        }
    }
}
