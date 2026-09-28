using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly SchoolDbContext _context;

        public StaffService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StaffResponseDto>> GetAllStaffAsync()
        {
            var staffs = await _context.dbsStaff
                .Include(m => m.Department)
                .Include(m => m.StaffSalary)
                .Include(m => m.StaffExperiences)
                .ToListAsync();

            return staffs.Select(MapToResponseDto);
        }

        public async Task<StaffResponseDto?> GetStaffByIdAsync(int id)
        {
            var staff = await _context.dbsStaff
                .Include(m => m.Department)
                .Include(m => m.StaffSalary)
                .Include(m => m.StaffExperiences)
                .FirstOrDefaultAsync(m => m.StaffId == id);

            return staff == null ? null : MapToResponseDto(staff);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, StaffResponseDto? CreatedStaff)> CreateStaffAsync(StaffCreateDto dto)
        {
            if (dto.DepartmentId.HasValue && dto.DepartmentId > 0)
            {
                var dept = await _context.dbsDepartment.FindAsync(dto.DepartmentId.Value);
                if (dept == null)
                {
                    return (false, $"Department with ID {dto.DepartmentId.Value} does not exist.", null);
                }
            }

            Gender? genderEnum = null;
            if (!string.IsNullOrWhiteSpace(dto.Gender) && Enum.TryParse<Gender>(dto.Gender, true, out var g))
            {
                genderEnum = g;
            }

            Designation? designationEnum = null;
            if (!string.IsNullOrWhiteSpace(dto.Designation) && Enum.TryParse<Designation>(dto.Designation, true, out var des))
            {
                designationEnum = des;
            }

            var entity = new Staff
            {
                StaffName = dto.StaffName,
                UniqueStaffAttendanceNumber = dto.UniqueStaffAttendanceNumber,
                Designation = designationEnum,
                DepartmentId = dto.DepartmentId,
                DOB = dto.DOB,
                Gender = genderEnum,
                ContactNumber1 = dto.ContactNumber1,
                Email = dto.Email,
                PermanentAddress = dto.PermanentAddress,
                TemporaryAddress = dto.TemporaryAddress,
                FatherName = dto.FatherName,
                MotherName = dto.MotherName,
                Qualifications = dto.Qualifications,
                JoiningDate = dto.JoiningDate,
                BankAccountName = dto.BankAccountName,
                BankAccountNumber = dto.BankAccountNumber,
                BankName = dto.BankName,
                BankBranch = dto.BankBranch,
                Status = dto.Status,
                ImagePath = dto.ImagePath
            };

            if (dto.StaffExperiences != null && dto.StaffExperiences.Count > 0)
            {
                entity.StaffExperiences = dto.StaffExperiences.Select(e => new StaffExperience
                {
                    CompanyName = e.CompanyName,
                    Designation = e.Designation,
                    JoiningDate = e.JoiningDate,
                    LeavingDate = e.LeavingDate,
                    Responsibilities = e.Responsibilities,
                    Achievements = e.Achievements
                }).ToList();
            }

            _context.dbsStaff.Add(entity);
            await _context.SaveChangesAsync();

            var created = await GetStaffByIdAsync(entity.StaffId);
            return (true, null, created);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, StaffResponseDto? Staff)> UpdateStaffAsync(int id, StaffUpdateDto dto)
        {
            if (id != dto.StaffId)
            {
                return (false, $"Provided URL ID '{id}' does not match StaffId '{dto.StaffId}'.", null);
            }

            var entity = await _context.dbsStaff
                .Include(s => s.StaffExperiences)
                .FirstOrDefaultAsync(s => s.StaffId == id);

            if (entity == null)
            {
                return (false, $"No staff member found with ID {id}.", null);
            }

            if (dto.DepartmentId.HasValue && dto.DepartmentId > 0)
            {
                var dept = await _context.dbsDepartment.FindAsync(dto.DepartmentId.Value);
                if (dept == null)
                {
                    return (false, $"Department with ID {dto.DepartmentId.Value} does not exist.", null);
                }
            }

            Gender? genderEnum = null;
            if (!string.IsNullOrWhiteSpace(dto.Gender) && Enum.TryParse<Gender>(dto.Gender, true, out var g))
            {
                genderEnum = g;
            }

            Designation? designationEnum = null;
            if (!string.IsNullOrWhiteSpace(dto.Designation) && Enum.TryParse<Designation>(dto.Designation, true, out var des))
            {
                designationEnum = des;
            }

            entity.StaffName = dto.StaffName;
            entity.UniqueStaffAttendanceNumber = dto.UniqueStaffAttendanceNumber;
            entity.Designation = designationEnum;
            entity.DepartmentId = dto.DepartmentId;
            entity.DOB = dto.DOB;
            entity.Gender = genderEnum;
            entity.ContactNumber1 = dto.ContactNumber1;
            entity.Email = dto.Email;
            entity.PermanentAddress = dto.PermanentAddress;
            entity.TemporaryAddress = dto.TemporaryAddress;
            entity.FatherName = dto.FatherName;
            entity.MotherName = dto.MotherName;
            entity.Qualifications = dto.Qualifications;
            entity.JoiningDate = dto.JoiningDate;
            entity.BankAccountName = dto.BankAccountName;
            entity.BankAccountNumber = dto.BankAccountNumber;
            entity.BankName = dto.BankName;
            entity.BankBranch = dto.BankBranch;
            entity.Status = dto.Status;
            entity.ImagePath = dto.ImagePath;

            await _context.SaveChangesAsync();

            var updated = await GetStaffByIdAsync(id);
            return (true, null, updated);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var staff = await _context.dbsStaff
                .Include(s => s.StaffExperiences)
                .FirstOrDefaultAsync(s => s.StaffId == id);

            if (staff == null) return false;

            if (staff.StaffExperiences != null && staff.StaffExperiences.Count > 0)
            {
                _context.dbsStaffExperience.RemoveRange(staff.StaffExperiences);
            }

            _context.dbsStaff.Remove(staff);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StaffExistsAsync(int id)
        {
            return await _context.dbsStaff.AnyAsync(e => e.StaffId == id);
        }

        private static StaffResponseDto MapToResponseDto(Staff s)
        {
            return new StaffResponseDto
            {
                StaffId = s.StaffId,
                StaffName = s.StaffName ?? string.Empty,
                UniqueStaffAttendanceNumber = s.UniqueStaffAttendanceNumber,
                Designation = s.Designation?.ToString(),
                DepartmentId = s.DepartmentId,
                DepartmentName = s.Department?.DepartmentName,
                DOB = s.DOB,
                Gender = s.Gender?.ToString(),
                ContactNumber1 = s.ContactNumber1,
                Email = s.Email,
                PermanentAddress = s.PermanentAddress,
                TemporaryAddress = s.TemporaryAddress,
                FatherName = s.FatherName,
                MotherName = s.MotherName,
                Qualifications = s.Qualifications,
                JoiningDate = s.JoiningDate,
                BankAccountName = s.BankAccountName,
                BankAccountNumber = s.BankAccountNumber,
                BankName = s.BankName,
                BankBranch = s.BankBranch,
                Status = s.Status,
                ImagePath = s.ImagePath,
                StaffExperiences = s.StaffExperiences?.Select(e => new StaffExperienceDto
                {
                    StaffExperienceId = e.StaffExperienceId,
                    CompanyName = e.CompanyName ?? string.Empty,
                    Designation = e.Designation ?? string.Empty,
                    JoiningDate = e.JoiningDate,
                    LeavingDate = e.LeavingDate,
                    Responsibilities = e.Responsibilities,
                    Achievements = e.Achievements
                }).ToList()
            };
        }
    }
}
