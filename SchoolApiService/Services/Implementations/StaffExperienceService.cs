using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class StaffExperienceService : IStaffExperienceService
    {
        private readonly SchoolDbContext _context;

        public StaffExperienceService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffExperienceDto>> GetAllStaffExperiencesAsync()
        {
            var exps = await _context.dbsStaffExperience.ToListAsync();
            return exps.Select(e => new StaffExperienceDto
            {
                StaffExperienceId = e.StaffExperienceId,
                CompanyName = e.CompanyName ?? string.Empty,
                Designation = e.Designation ?? string.Empty,
                JoiningDate = e.JoiningDate,
                LeavingDate = e.LeavingDate,
                Responsibilities = e.Responsibilities,
                Achievements = e.Achievements
            });
        }

        public async Task<StaffExperienceDto?> GetStaffExperienceByIdAsync(int id)
        {
            var e = await _context.dbsStaffExperience.FindAsync(id);
            if (e == null) return null;

            return new StaffExperienceDto
            {
                StaffExperienceId = e.StaffExperienceId,
                CompanyName = e.CompanyName ?? string.Empty,
                Designation = e.Designation ?? string.Empty,
                JoiningDate = e.JoiningDate,
                LeavingDate = e.LeavingDate,
                Responsibilities = e.Responsibilities,
                Achievements = e.Achievements
            };
        }

        public async Task<StaffExperienceDto> CreateStaffExperienceAsync(StaffExperienceDto dto)
        {
            var entity = new StaffExperience
            {
                CompanyName = dto.CompanyName,
                Designation = dto.Designation,
                JoiningDate = dto.JoiningDate,
                LeavingDate = dto.LeavingDate,
                Responsibilities = dto.Responsibilities,
                Achievements = dto.Achievements
            };

            _context.dbsStaffExperience.Add(entity);
            await _context.SaveChangesAsync();
            dto.StaffExperienceId = entity.StaffExperienceId;
            return dto;
        }

        public async Task<bool> UpdateStaffExperienceAsync(int id, StaffExperienceDto dto)
        {
            if (id != dto.StaffExperienceId) return false;

            var entity = await _context.dbsStaffExperience.FindAsync(id);
            if (entity == null) return false;

            entity.CompanyName = dto.CompanyName;
            entity.Designation = dto.Designation;
            entity.JoiningDate = dto.JoiningDate;
            entity.LeavingDate = dto.LeavingDate;
            entity.Responsibilities = dto.Responsibilities;
            entity.Achievements = dto.Achievements;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteStaffExperienceAsync(int id)
        {
            var exp = await _context.dbsStaffExperience.FindAsync(id);
            if (exp == null) return false;

            _context.dbsStaffExperience.Remove(exp);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StaffExperienceExistsAsync(int id)
        {
            return await _context.dbsStaffExperience.AnyAsync(e => e.StaffExperienceId == id);
        }
    }
}
