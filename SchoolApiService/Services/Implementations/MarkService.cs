using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class MarkService : IMarkService
    {
        private readonly SchoolDbContext _context;

        public MarkService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Mark>> GetAllMarksAsync()
        {
            return await _context.dbsMark
                .Include(m => m.Staff)
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .ToListAsync();
        }

        public async Task<Mark?> GetMarkByIdAsync(int id)
        {
            return await _context.dbsMark
                .Include(m => m.Staff)
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .FirstOrDefaultAsync(m => m.MarkId == id);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, object? Result)> CreateMarkAsync(Mark mark)
        {
            try
            {
                var existingStaff = await _context.dbsStaff.FindAsync(mark.StaffId);
                if (existingStaff == null)
                {
                    return (false, "Invalid / Not given StaffId. Please provide a valid StaffId.", null);
                }

                var existingStudent = await _context.dbsStudent.FindAsync(mark.StudentId);
                if (existingStudent == null)
                {
                    return (false, "Invalid / Not given StudentId. Please provide a valid StudentId.", null);
                }

                var existingSubject = await _context.dbsSubject.FindAsync(mark.SubjectId);
                if (existingSubject == null)
                {
                    return (false, "Invalid / Not given SubjectId. Please provide a valid SubjectId.", null);
                }

                _context.dbsMark.Add(mark);
                await _context.SaveChangesAsync();

                var result = new
                {
                    mark = mark,
                    message = $"You have just inserted ID: {mark.MarkId}"
                };

                return (true, null, result);
            }
            catch (Exception ex)
            {
                return (false, $"Ooops!!! Errrrrors!!: {ex.Message}", null);
            }
        }

        public async Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateMarkAsync(int id, Mark mark)
        {
            if (id != mark.MarkId)
            {
                return (false, "The ID in the request body does not match the ID in the route parameter.", false);
            }

            if (mark.StaffId != null && !await _context.dbsStaff.AnyAsync(s => s.StaffId == mark.StaffId))
            {
                return (false, $"Invalid StaffId: {mark.StaffId}. The specified staff does not exist in the database.", false);
            }

            if (mark.StudentId != null && !await _context.dbsStudent.AnyAsync(s => s.StudentId == mark.StudentId))
            {
                return (false, $"Invalid StudentId: {mark.StudentId}. The specified student does not exist in the database.", false);
            }

            if (mark.SubjectId != null && !await _context.dbsSubject.AnyAsync(s => s.SubjectId == mark.SubjectId))
            {
                return (false, $"Invalid SubjectId: {mark.SubjectId}. The specified subject does not exist in the database.", false);
            }

            _context.Entry(mark).Property(p => p.TotalMarks).IsModified = mark.TotalMarks != null;
            _context.Entry(mark).Property(p => p.PassMarks).IsModified = mark.PassMarks != null;
            _context.Entry(mark).Property(p => p.ObtainedScore).IsModified = mark.ObtainedScore != null;
            _context.Entry(mark).Property(p => p.Grade).IsModified = mark.Grade != null;
            _context.Entry(mark).Property(p => p.PassStatus).IsModified = mark.PassStatus != null;
            _context.Entry(mark).Property(p => p.MarkEntryDate).IsModified = mark.MarkEntryDate != null;
            _context.Entry(mark).Property(p => p.Feedback).IsModified = mark.Feedback != null;
            _context.Entry(mark).Property(p => p.StaffId).IsModified = mark.StaffId != null;
            _context.Entry(mark).Property(p => p.StudentId).IsModified = mark.StudentId != null;
            _context.Entry(mark).Property(p => p.SubjectId).IsModified = mark.SubjectId != null;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await MarkExistsAsync(id))
                {
                    return (false, "Mark not found.", true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteMarkAsync(int id)
        {
            var mark = await _context.dbsMark.FindAsync(id);
            if (mark == null)
            {
                return false;
            }

            _context.dbsMark.Remove(mark);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkExistsAsync(int id)
        {
            return await _context.dbsMark.AnyAsync(e => e.MarkId == id);
        }
    }
}
