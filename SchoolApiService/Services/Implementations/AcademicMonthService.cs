using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
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

        public async Task<IEnumerable<AcademicMonthDto>> GetAllAsync()
        {
            var months = await _context.dbsAcademicMonths.ToListAsync();
            return months.Select(m => new AcademicMonthDto
            {
                MonthId = m.MonthId,
                MonthName = m.MonthName ?? string.Empty
            });
        }

        public async Task<AcademicMonthDto?> GetByIdAsync(int id)
        {
            var m = await _context.dbsAcademicMonths.FindAsync(id);
            if (m == null) return null;
            return new AcademicMonthDto
            {
                MonthId = m.MonthId,
                MonthName = m.MonthName ?? string.Empty
            };
        }

        public async Task<AcademicMonthDto> CreateAsync(AcademicMonthDto dto)
        {
            var entity = new AcademicMonth
            {
                MonthName = dto.MonthName
            };
            _context.dbsAcademicMonths.Add(entity);
            await _context.SaveChangesAsync();
            dto.MonthId = entity.MonthId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, AcademicMonthDto dto)
        {
            if (id != dto.MonthId) return false;

            var entity = await _context.dbsAcademicMonths.FindAsync(id);
            if (entity == null) return false;

            entity.MonthName = dto.MonthName;
            await _context.SaveChangesAsync();
            return true;
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
