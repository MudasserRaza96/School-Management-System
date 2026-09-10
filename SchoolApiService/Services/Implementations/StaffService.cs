using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly SchoolDbContext _context;

        public StaffService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Staff>> GetAllStaffAsync()
        {
            return await _context.dbsStaff
                .Include(m => m.Department)
                .Include(m => m.StaffSalary)
                .Include(m => m.StaffExperiences)
                .ToListAsync();
        }

        public async Task<Staff?> GetStaffByIdAsync(int id)
        {
            return await _context.dbsStaff
                .Include(m => m.Department)
                .Include(m => m.StaffSalary)
                .Include(m => m.StaffExperiences)
                .FirstOrDefaultAsync(m => m.StaffId == id);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, Staff? CreatedStaff)> CreateStaffAsync(Staff staff)
        {
            if (staff.DepartmentId != null)
            {
                staff.Department = await _context.dbsDepartment.FindAsync(staff.DepartmentId);
                if (staff.Department == null)
                {
                    return (false, "Invalid DepartmentId", null);
                }
            }

            if (staff.StaffSalaryId != null)
            {
                staff.StaffSalary = await _context.dbsStaffSalary.FindAsync(staff.StaffSalaryId);
                if (staff.StaffSalary == null)
                {
                    return (false, "Invalid StaffSalaryId", null);
                }
            }

            if (staff.ImageUpload?.ImageData != null)
            {
                staff.ImagePath = staff.ImageUpload.ImageData;
            }

            if (staff.StaffExperiences != null && staff.StaffExperiences.Any())
            {
                foreach (var experience in staff.StaffExperiences)
                {
                    _context.dbsStaffExperience.Add(experience);
                }
            }

            _context.dbsStaff.Add(staff);

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, staff);
            }
            catch (DbUpdateException)
            {
                return (false, "Unable to save changes. Please try again.", null);
            }
        }

        public async Task<(bool Succeeded, string? ErrorMessage, bool ConcurrencyError)> UpdateStaffAsync(int id, Staff staff)
        {
            if (id != staff.StaffId)
            {
                return (false, "Invalid StaffId", false);
            }

            if (staff.StaffSalaryId != null)
            {
                staff.StaffSalary = await _context.dbsStaffSalary.FindAsync(staff.StaffSalaryId);
                if (staff.StaffSalary == null)
                {
                    return (false, "Invalid StaffSalaryId", false);
                }
            }

            if (staff.DepartmentId != null)
            {
                staff.Department = await _context.dbsDepartment.FindAsync(staff.DepartmentId);
                if (staff.Department == null)
                {
                    return (false, "Invalid DepartmentId", false);
                }
            }

            if (staff.ImageUpload?.ImageData != null)
            {
                staff.ImagePath = staff.ImageUpload.ImageData;
            }

            if (staff.StaffExperiences != null && staff.StaffExperiences.Any())
            {
                foreach (var experience in staff.StaffExperiences)
                {
                    if (experience.StaffExperienceId == 0)
                    {
                        _context.dbsStaffExperience.Add(experience);
                    }
                    else
                    {
                        _context.Entry(experience).State = EntityState.Modified;
                    }
                }
            }

            _context.Entry(staff).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, null, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StaffExistsAsync(id))
                {
                    return (false, null, true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.dbsStaff
                .Include(s => s.StaffExperiences)
                .FirstOrDefaultAsync(s => s.StaffId == id);

            if (staff == null)
            {
                return false;
            }

            _context.dbsStaffExperience.RemoveRange(staff.StaffExperiences);
            _context.dbsStaff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StaffExistsAsync(int id)
        {
            return await _context.dbsStaff.AnyAsync(e => e.StaffId == id);
        }
    }
}
