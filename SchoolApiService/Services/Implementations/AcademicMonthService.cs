using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class AcademicMonthService : IAcademicMonthService
    {
        private readonly SchoolDbContext _context;

        public AcademicMonthService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AcademicMonth>> GetAllAsync()
        {
            return await _context.dbsAcademicMonths.ToListAsync();
        }

        public async Task<AcademicMonth?> GetByIdAsync(int id)
        {
            return await _context.dbsAcademicMonths.FindAsync(id);
        }

        public async Task<AcademicMonth> CreateAsync(AcademicMonth academicMonth)
        {
            _context.dbsAcademicMonths.Add(academicMonth);
            await _context.SaveChangesAsync();
            return academicMonth;
        }

        public async Task<bool> UpdateAsync(int id, AcademicMonth academicMonth)
        {
            if (id != academicMonth.MonthId) return false;

            _context.Entry(academicMonth).State = EntityState.Modified;

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
            var academicMonth = await _context.dbsAcademicMonths.FindAsync(id);
            if (academicMonth == null) return false;

            _context.dbsAcademicMonths.Remove(academicMonth);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsAcademicMonths.AnyAsync(e => e.MonthId == id);
        }
    }
}
