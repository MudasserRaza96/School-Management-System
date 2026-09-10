using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class StandardService : IStandardService
    {
        private readonly SchoolDbContext _context;

        public StandardService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Standard>> GetAllAsync()
        {
            return await _context.dbsStandard
                .Include(m => m.Subjects)
                .Include(m => m.ExamScheduleStandards)
                .Include(m => m.Students)
                .ToListAsync();
        }

        public async Task<Standard?> GetByIdAsync(int id)
        {
            return await _context.dbsStandard
                .Include(m => m.Subjects)
                .Include(m => m.ExamScheduleStandards)
                .Include(m => m.Students)
                .FirstOrDefaultAsync(m => m.StandardId == id);
        }

        public async Task<Standard> CreateAsync(Standard standard)
        {
            _context.dbsStandard.Add(standard);
            await _context.SaveChangesAsync();
            return standard;
        }

        public async Task<bool> UpdateAsync(int id, Standard standard)
        {
            if (id != standard.StandardId) return false;

            _context.Entry(standard).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExistsAsync(id))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(int id)
        {
            var standard = await _context.dbsStandard.FindAsync(id);
            if (standard == null)
            {
                return (false, null);
            }

            var hasStudents = await _context.dbsStudent.AnyAsync(s => s.StandardId == id);
            if (hasStudents)
            {
                return (false, "Cannot delete Standard with associated Students.");
            }

            _context.dbsStandard.Remove(standard);
            await _context.SaveChangesAsync();
            return (true, null);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsStandard.AnyAsync(e => e.StandardId == id);
        }
    }
}
