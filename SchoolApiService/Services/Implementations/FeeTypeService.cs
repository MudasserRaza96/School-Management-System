using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class FeeTypeService : IFeeTypeService
    {
        private readonly SchoolDbContext _context;

        public FeeTypeService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<FeeType>> GetAllAsync()
        {
            return await _context.dbsFeeType.ToListAsync();
        }

        public async Task<FeeType?> GetByIdAsync(int id)
        {
            return await _context.dbsFeeType.FindAsync(id);
        }

        public async Task<FeeType> CreateAsync(FeeType feeType)
        {
            _context.dbsFeeType.Add(feeType);
            await _context.SaveChangesAsync();
            return feeType;
        }

        public async Task<bool> UpdateAsync(int id, FeeType feeType)
        {
            if (id != feeType.FeeTypeId) return false;

            _context.Entry(feeType).State = EntityState.Modified;

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
            var feeType = await _context.dbsFeeType.FindAsync(id);
            if (feeType == null) return false;

            _context.dbsFeeType.Remove(feeType);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsFeeType.AnyAsync(e => e.FeeTypeId == id);
        }
    }
}
