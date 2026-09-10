using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class ExamTypeService : IExamTypeService
    {
        private readonly SchoolDbContext _context;

        public ExamTypeService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamType>> GetAllExamTypesAsync()
        {
            return await _context.dbsExamType.ToListAsync();
        }

        public async Task<ExamType?> GetExamTypeByIdAsync(int id)
        {
            return await _context.dbsExamType.FindAsync(id);
        }

        public async Task<ExamType> CreateExamTypeAsync(ExamType examType)
        {
            _context.dbsExamType.Add(examType);
            await _context.SaveChangesAsync();
            return examType;
        }

        public async Task<(bool Succeeded, bool ConcurrencyError)> UpdateExamTypeAsync(int id, ExamType examType)
        {
            if (id != examType.ExamTypeId)
            {
                return (false, false);
            }

            _context.Entry(examType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExamTypeExistsAsync(id))
                {
                    return (false, true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteExamTypeAsync(int id)
        {
            var examType = await _context.dbsExamType.FindAsync(id);
            if (examType == null)
            {
                return false;
            }

            _context.dbsExamType.Remove(examType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExamTypeExistsAsync(int id)
        {
            return await _context.dbsExamType.AnyAsync(e => e.ExamTypeId == id);
        }
    }
}
