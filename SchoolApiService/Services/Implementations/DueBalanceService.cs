using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class DueBalanceService : IDueBalanceService
    {
        private readonly SchoolDbContext _context;

        public DueBalanceService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DueBalanceDto>> GetAllAsync()
        {
            var list = await _context.dbsDueBalance.Include(d => d.Student).ToListAsync();
            return list.Select(d => new DueBalanceDto
            {
                DueBalanceId = d.DueBalanceId,
                StudentId = d.StudentId ?? 0,
                StudentName = d.Student?.StudentName,
                DueAmount = d.DueBalanceAmount ?? 0
            });
        }

        public async Task<DueBalanceDto?> GetByIdAsync(int id)
        {
            var d = await _context.dbsDueBalance.Include(d => d.Student).FirstOrDefaultAsync(d => d.DueBalanceId == id);
            if (d == null) return null;

            return new DueBalanceDto
            {
                DueBalanceId = d.DueBalanceId,
                StudentId = d.StudentId ?? 0,
                StudentName = d.Student?.StudentName,
                DueAmount = d.DueBalanceAmount ?? 0
            };
        }

        public async Task<DueBalanceDto> CreateAsync(DueBalanceDto dto)
        {
            var entity = new DueBalance
            {
                StudentId = dto.StudentId,
                DueBalanceAmount = dto.DueAmount
            };

            _context.dbsDueBalance.Add(entity);
            await _context.SaveChangesAsync();
            dto.DueBalanceId = entity.DueBalanceId;
            return dto;
        }

        public async Task<bool> UpdateAsync(int id, DueBalanceDto dto)
        {
            if (id != dto.DueBalanceId) return false;

            var entity = await _context.dbsDueBalance.FindAsync(id);
            if (entity == null) return false;

            entity.StudentId = dto.StudentId;
            entity.DueBalanceAmount = dto.DueAmount;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var d = await _context.dbsDueBalance.FindAsync(id);
            if (d == null) return false;

            _context.dbsDueBalance.Remove(d);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsDueBalance.AnyAsync(e => e.DueBalanceId == id);
        }
    }
}
