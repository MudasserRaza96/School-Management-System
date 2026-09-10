using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class SubjectService : ISubjectService
    {
        private readonly SchoolDbContext _context;

        public SubjectService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Subject>> GetAllAsync()
        {
            return await _context.dbsSubject.Include(s => s.Standard).ToListAsync();
        }

        public async Task<Subject?> GetByIdAsync(int id)
        {
            return await _context.dbsSubject.FindAsync(id);
        }

        public async Task<Subject> CreateAsync(Subject subject)
        {
            _context.dbsSubject.Add(subject);
            await _context.SaveChangesAsync();
            return subject;
        }

        public async Task<bool> UpdateAsync(int id, Subject subject)
        {
            if (id != subject.SubjectId) return false;

            _context.Entry(subject).State = EntityState.Modified;

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

        public async Task<bool> DeleteAsync(int id)
        {
            var subject = await _context.dbsSubject.FindAsync(id);
            if (subject == null) return false;

            _context.dbsSubject.Remove(subject);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsSubject.AnyAsync(e => e.SubjectId == id);
        }
    }
}
