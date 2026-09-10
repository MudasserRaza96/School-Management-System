using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class StaffExperienceService : IStaffExperienceService
    {
        private readonly SchoolDbContext _context;

        public StaffExperienceService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffExperience>> GetAllStaffExperiencesAsync()
        {
            return await _context.dbsStaffExperience.ToListAsync();
        }

        public async Task<StaffExperience?> GetStaffExperienceByIdAsync(int id)
        {
            return await _context.dbsStaffExperience.FindAsync(id);
        }

        public async Task<StaffExperience> CreateStaffExperienceAsync(StaffExperience staffExperience)
        {
            _context.dbsStaffExperience.Add(staffExperience);
            await _context.SaveChangesAsync();
            return staffExperience;
        }

        public async Task<(bool Succeeded, bool ConcurrencyError)> UpdateStaffExperienceAsync(int id, StaffExperience staffExperience)
        {
            if (id != staffExperience.StaffExperienceId)
            {
                return (false, false);
            }

            _context.Entry(staffExperience).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StaffExperienceExistsAsync(id))
                {
                    return (false, true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteStaffExperienceAsync(int id)
        {
            var staffExperience = await _context.dbsStaffExperience.FindAsync(id);
            if (staffExperience == null)
            {
                return false;
            }

            _context.dbsStaffExperience.Remove(staffExperience);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StaffExperienceExistsAsync(int id)
        {
            return await _context.dbsStaffExperience.AnyAsync(e => e.StaffExperienceId == id);
        }
    }
}
