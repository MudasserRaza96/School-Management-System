using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
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

        public async Task<IEnumerable<DueBalance>> GetAllAsync()
        {
            return await _context.dbsDueBalance.ToListAsync();
        }

        public async Task<DueBalance?> GetByIdAsync(int id)
        {
            return await _context.dbsDueBalance.FindAsync(id);
        }

        public async Task<DueBalance> CreateAsync(DueBalance dueBalance)
        {
            _context.dbsDueBalance.Add(dueBalance);
            await _context.SaveChangesAsync();
            return dueBalance;
        }

        public async Task<bool> UpdateAsync(int id, DueBalance dueBalance)
        {
            if (id != dueBalance.DueBalanceId) return false;

            _context.Entry(dueBalance).State = EntityState.Modified;

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
            var dueBalance = await _context.dbsDueBalance.FindAsync(id);
            if (dueBalance == null) return false;

            _context.dbsDueBalance.Remove(dueBalance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.dbsDueBalance.AnyAsync(e => e.DueBalanceId == id);
        }
    }
}
