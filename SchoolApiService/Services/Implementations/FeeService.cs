using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class FeeService : IFeeService
    {
        private readonly SchoolDbContext _context;

        public FeeService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeeDto>> GetAllFeesAsync()
        {
            var fees = await _context.fees.Include(f => f.feeType).ToListAsync();
            return fees.Select(f => new FeeDto
            {
                FeeId = f.FeeId,
                FeeName = f.feeType?.TypeName ?? $"Fee {f.FeeId}",
                Amount = f.Amount
            });
        }

        public async Task<FeeDto?> GetFeeByIdAsync(int id)
        {
            var f = await _context.fees.Include(x => x.feeType).FirstOrDefaultAsync(x => x.FeeId == id);
            if (f == null) return null;

            return new FeeDto
            {
                FeeId = f.FeeId,
                FeeName = f.feeType?.TypeName ?? $"Fee {f.FeeId}",
                Amount = f.Amount
            };
        }

        public async Task<FeeDto> CreateFeeAsync(FeeDto dto)
        {
            var entity = new Fee
            {
                Amount = dto.Amount
            };

            _context.fees.Add(entity);
            await _context.SaveChangesAsync();
            dto.FeeId = entity.FeeId;
            return dto;
        }

        public async Task<bool> UpdateFeeAsync(int id, FeeDto dto)
        {
            if (id != dto.FeeId) return false;

            var entity = await _context.fees.FindAsync(id);
            if (entity == null) return false;

            entity.Amount = dto.Amount;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFeeAsync(int id)
        {
            var fee = await _context.fees.FindAsync(id);
            if (fee == null) return false;

            _context.fees.Remove(fee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> FeeExistsAsync(int id)
        {
            return await _context.fees.AnyAsync(e => e.FeeId == id);
        }
    }
}
