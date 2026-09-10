using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class StaffSalaryService : IStaffSalaryService
    {
        private readonly SchoolDbContext _context;

        public StaffSalaryService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffSalary>> GetAllStaffSalariesAsync()
        {
            return await _context.dbsStaffSalary.ToListAsync();
        }

        public async Task<StaffSalary?> GetStaffSalaryByIdAsync(int id)
        {
            return await _context.dbsStaffSalary.FindAsync(id);
        }

        public async Task<StaffSalary> CreateStaffSalaryAsync(StaffSalary staffSalary)
        {
            _context.dbsStaffSalary.Add(staffSalary);
            await _context.SaveChangesAsync();
            return staffSalary;
        }

        public async Task<(bool Succeeded, bool ConcurrencyError)> UpdateStaffSalaryAsync(int id, StaffSalary staffSalary)
        {
            if (id != staffSalary.StaffSalaryId)
            {
                return (false, false);
            }

            _context.Entry(staffSalary).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StaffSalaryExistsAsync(id))
                {
                    return (false, true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteStaffSalaryAsync(int id)
        {
            var staffSalary = await _context.dbsStaffSalary.FindAsync(id);
            if (staffSalary == null)
            {
                return false;
            }

            _context.dbsStaffSalary.Remove(staffSalary);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StaffSalaryExistsAsync(int id)
        {
            return await _context.dbsStaffSalary.AnyAsync(e => e.StaffSalaryId == id);
        }
    }
}
