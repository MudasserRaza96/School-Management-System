using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
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

        public async Task<IEnumerable<Fee>> GetAllAsync()
        {
            return await _context.fees
                .Include(fp => fp.feeType)
                .Include(fp => fp.standard)
                .ToListAsync();
        }

        public async Task<Fee?> GetByIdAsync(int id)
        {
            return await _context.fees.FindAsync(id);
        }

        public async Task<Fee> CreateAsync(Fee fee)
        {
            _context.fees.Add(fee);
            await _context.SaveChangesAsync();
            return fee;
        }

        public async Task<bool> UpdateAsync(int id, Fee fee)
        {
            if (id != fee.FeeId) return false;

            _context.Entry(fee).State = EntityState.Modified;

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
            var fee = await _context.fees.FindAsync(id);
            if (fee == null) return false;

            _context.fees.Remove(fee);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.fees.AnyAsync(e => e.FeeId == id);
        }
    }
}
