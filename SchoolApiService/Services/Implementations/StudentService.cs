using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly SchoolDbContext _context;

        public StudentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Student>> GetAllStudentsAsync()
        {
            return await _context.dbsStudent
                .Include(s => s.Standard)
                .ToListAsync();
        }

        public async Task<Student?> GetStudentByIdAsync(int id)
        {
            return await _context.dbsStudent
                .Include(s => s.Standard)
                .FirstOrDefaultAsync(s => s.StudentId == id);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateStudentAsync(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return (false, "Invalid StudentId", false);
            }

            if (student.StandardId != null)
            {
                student.Standard = await _context.dbsStandard.FindAsync(student.StandardId);
                if (student.Standard == null)
                {
                    return (false, "Invalid StandardId", false);
                }
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StudentExistsAsync(id))
                {
                    return (false, null, true);
                }
                throw;
            }
        }

        public async Task<(bool Succeeded, string? ErrorMessage, Student? CreatedStudent)> CreateStudentAsync(Student student)
        {
            if (student.StandardId != null)
            {
                student.Standard = await _context.dbsStandard.FindAsync(student.StandardId);
                if (student.Standard == null)
                {
                    return (false, "Invalid StandardId", null);
                }
            }

            if (student.ImageUpload?.ImageData != null)
            {
                student.ImagePath = student.ImageUpload.ImageData;
            }

            _context.dbsStudent.Add(student);

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, student);
            }
            catch (DbUpdateException)
            {
                return (false, "Unable to save changes. Please try again.", null);
            }
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.dbsStudent.FindAsync(id);
            if (student == null)
            {
                return false;
            }

            _context.dbsStudent.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _context.dbsStudent.AnyAsync(e => e.StudentId == id);
        }
    }
}
