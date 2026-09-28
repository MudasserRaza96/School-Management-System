using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
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

        public async Task<IEnumerable<StandardDto>> GetAllAsync()
        {
            var stds = await _context.dbsStandard.ToListAsync();
            return stds.Select(s => new StandardDto
            {
                StandardId = s.StandardId,
                StandardName = s.StandardName ?? string.Empty
            });
        }

        public async Task<StandardDto?> GetByIdAsync(int id)
        {
            var s = await _context.dbsStandard.FindAsync(id);
            if (s == null) return null;
            return new StandardDto
            {
                StandardId = s.StandardId,
                StandardName = s.StandardName ?? string.Empty
            };
        }

        public async Task<StandardDto> CreateAsync(StandardDto dto)
        {
            var entity = new Standard
            {
                StandardName = dto.StandardName
            };
            _context.dbsStandard.Add(entity);
            await _context.SaveChangesAsync();
            dto.StandardId = entity.StandardId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, StandardDto dto)
        {
            if (id != dto.StandardId) return false;

            var entity = await _context.dbsStandard.FindAsync(id);
            if (entity == null) return false;

            entity.StandardName = dto.StandardName;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var std = await _context.dbsStandard.FindAsync(id);
            if (std == null) return false;

            _context.dbsStandard.Remove(std);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsStandard.AnyAsync(e => e.StandardId == id);
        }
    }
}
