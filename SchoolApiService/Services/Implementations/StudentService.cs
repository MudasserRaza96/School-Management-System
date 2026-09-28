using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.DTOs;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly SchoolDbContext _context;

        public StudentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StudentResponseDto>> GetAllStudentsAsync()
        {
            var students = await _context.dbsStudent
                .Include(s => s.Standard)
                .ToListAsync();

            return students.Select(MapToResponseDto);
        }

        public async Task<StudentResponseDto?> GetStudentByIdAsync(int id)
        {
            var student = await _context.dbsStudent
                .Include(s => s.Standard)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            return student == null ? null : MapToResponseDto(student);
        }

        public async Task<(bool Succeeded, string? ErrorMessage, StudentResponseDto? Student)> UpdateStudentAsync(int id, StudentUpdateDto dto)
        {
            if (id != dto.StudentId)
            {
                return (false, $"Provided ID '{id}' does not match StudentId '{dto.StudentId}'.", null);
            }

            var entity = await _context.dbsStudent
                .Include(s => s.Standard)
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (entity == null)
            {
                return (false, $"No student found with ID {id}.", null);
            }

            if (dto.StandardId > 0)
            {
                var standard = await _context.dbsStandard.FindAsync(dto.StandardId);
                if (standard == null)
                {
                    return (false, $"Standard with ID {dto.StandardId} does not exist.", null);
                }
                entity.StandardId = dto.StandardId;
                entity.Standard = standard;
            }

            GenderList? genderEnum = null;
            if (!string.IsNullOrWhiteSpace(dto.StudentGender) && Enum.TryParse<GenderList>(dto.StudentGender, true, out var parsedGender))
            {
                genderEnum = parsedGender;
            }

            entity.AdmissionNo = dto.AdmissionNo;
            entity.EnrollmentNo = dto.EnrollmentNo;
            entity.UniqueStudentAttendanceNumber = dto.UniqueStudentAttendanceNumber;
            entity.StudentName = dto.StudentName;
            entity.StudentDOB = dto.StudentDOB;
            entity.StudentGender = genderEnum;
            entity.StudentReligion = dto.StudentReligion;
            entity.StudentBloodGroup = dto.StudentBloodGroup;
            entity.StudentNationality = dto.StudentNationality;
            entity.StudentNIDNumber = dto.StudentNIDNumber;
            entity.StudentContactNumber1 = dto.StudentContactNumber1;
            entity.StudentContactNumber2 = dto.StudentContactNumber2;
            entity.StudentEmail = dto.StudentEmail;
            entity.ImagePath = dto.ImagePath;
            entity.FatherName = dto.FatherName;
            entity.FatherNID = dto.FatherNID;
            entity.FatherContactNumber = dto.FatherContactNumber;
            entity.MotherName = dto.MotherName;
            entity.MotherNID = dto.MotherNID;
            entity.MotherContactNumber = dto.MotherContactNumber;
            entity.LocalGuardianName = dto.LocalGuardianName;
            entity.LocalGuardianContactNumber = dto.LocalGuardianContactNumber;
            entity.TemporaryAddress = dto.TemporaryAddress;
            entity.PermanentAddress = dto.PermanentAddress;

            await _context.SaveChangesAsync();
            return (true, null, MapToResponseDto(entity));
        }

        public async Task<(bool Succeeded, string? ErrorMessage, StudentResponseDto? CreatedStudent)> CreateStudentAsync(StudentCreateDto dto)
        {
            if (dto.StandardId > 0)
            {
                var standard = await _context.dbsStandard.FindAsync(dto.StandardId);
                if (standard == null)
                {
                    return (false, $"Standard with ID {dto.StandardId} does not exist.", null);
                }
            }

            if (!string.IsNullOrWhiteSpace(dto.StudentEmail))
            {
                var emailExists = await _context.dbsStudent.AnyAsync(s => s.StudentEmail == dto.StudentEmail);
                if (emailExists)
                {
                    return (false, $"A student with email '{dto.StudentEmail}' already exists.", null);
                }
            }

            GenderList? genderEnum = null;
            if (!string.IsNullOrWhiteSpace(dto.StudentGender) && Enum.TryParse<GenderList>(dto.StudentGender, true, out var parsedGender))
            {
                genderEnum = parsedGender;
            }

            var entity = new Student
            {
                AdmissionNo = dto.AdmissionNo,
                EnrollmentNo = dto.EnrollmentNo,
                UniqueStudentAttendanceNumber = dto.UniqueStudentAttendanceNumber,
                StudentName = dto.StudentName,
                StudentDOB = dto.StudentDOB,
                StudentGender = genderEnum,
                StudentReligion = dto.StudentReligion,
                StudentBloodGroup = dto.StudentBloodGroup,
                StudentNationality = dto.StudentNationality,
                StudentNIDNumber = dto.StudentNIDNumber,
                StudentContactNumber1 = dto.StudentContactNumber1,
                StudentContactNumber2 = dto.StudentContactNumber2,
                StudentEmail = dto.StudentEmail,
                StandardId = dto.StandardId,
                ImagePath = dto.ImagePath,
                FatherName = dto.FatherName,
                FatherNID = dto.FatherNID,
                FatherContactNumber = dto.FatherContactNumber,
                MotherName = dto.MotherName,
                MotherNID = dto.MotherNID,
                MotherContactNumber = dto.MotherContactNumber,
                LocalGuardianName = dto.LocalGuardianName,
                LocalGuardianContactNumber = dto.LocalGuardianContactNumber,
                TemporaryAddress = dto.TemporaryAddress,
                PermanentAddress = dto.PermanentAddress
            };

            _context.dbsStudent.Add(entity);
            await _context.SaveChangesAsync();

            var createdEntity = await _context.dbsStudent
                .Include(s => s.Standard)
                .FirstOrDefaultAsync(s => s.StudentId == entity.StudentId) ?? entity;

            return (true, null, MapToResponseDto(createdEntity));
        }

        public async Task<bool> DeleteStudentAsync(int id)
        {
            var student = await _context.dbsStudent.FindAsync(id);
            if (student == null) return false;

            _context.dbsStudent.Remove(student);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> StudentExistsAsync(int id)
        {
            return await _context.dbsStudent.AnyAsync(e => e.StudentId == id);
        }

        private static StudentResponseDto MapToResponseDto(Student s)
        {
            return new StudentResponseDto
            {
                StudentId = s.StudentId,
                AdmissionNo = s.AdmissionNo,
                EnrollmentNo = s.EnrollmentNo,
                UniqueStudentAttendanceNumber = s.UniqueStudentAttendanceNumber,
                StudentName = s.StudentName ?? string.Empty,
                StudentDOB = s.StudentDOB,
                StudentGender = s.StudentGender?.ToString(),
                StudentReligion = s.StudentReligion,
                StudentBloodGroup = s.StudentBloodGroup,
                StudentNationality = s.StudentNationality,
                StudentNIDNumber = s.StudentNIDNumber,
                StudentContactNumber1 = s.StudentContactNumber1,
                StudentContactNumber2 = s.StudentContactNumber2,
                StudentEmail = s.StudentEmail ?? string.Empty,
                StandardId = s.StandardId ?? 0,
                StandardName = s.Standard?.StandardName,
                ImagePath = s.ImagePath,
                FatherName = s.FatherName,
                FatherNID = s.FatherNID,
                FatherContactNumber = s.FatherContactNumber,
                MotherName = s.MotherName,
                MotherNID = s.MotherNID,
                MotherContactNumber = s.MotherContactNumber,
                LocalGuardianName = s.LocalGuardianName,
                LocalGuardianContactNumber = s.LocalGuardianContactNumber,
                TemporaryAddress = s.TemporaryAddress,
                PermanentAddress = s.PermanentAddress
            };
        }
    }
}
