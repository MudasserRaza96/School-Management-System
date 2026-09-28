namespace SchoolApiService.DTOs
{
    public class StaffExperienceDto
    {
        public int StaffExperienceId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public DateTime JoiningDate { get; set; }
        public DateTime? LeavingDate { get; set; }
        public string? Responsibilities { get; set; }
        public string? Achievements { get; set; }
    }

    public class StaffSalaryDto
    {
        public int StaffSalaryId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public decimal BasicSalary { get; set; }
        public decimal NetSalary { get; set; }
    }

    public class StaffCreateDto
    {
        public string StaffName { get; set; } = string.Empty;
        public int UniqueStaffAttendanceNumber { get; set; } = 200;
        public string? Designation { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime? DOB { get; set; }
        public string? Gender { get; set; }
        public string? ContactNumber1 { get; set; }
        public string? Email { get; set; }
        public string? PermanentAddress { get; set; }
        public string? TemporaryAddress { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }
        public string? Qualifications { get; set; }
        public DateTime? JoiningDate { get; set; }
        public string? BankAccountName { get; set; }
        public int? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankBranch { get; set; }
        public string? Status { get; set; }
        public string? ImagePath { get; set; }
        public List<StaffExperienceDto>? StaffExperiences { get; set; }
    }

    public class StaffUpdateDto : StaffCreateDto
    {
        public int StaffId { get; set; }
    }

    public class StaffResponseDto : StaffCreateDto
    {
        public int StaffId { get; set; }
        public string? DepartmentName { get; set; }
    }

    public class EmployeeTypeDto
    {
        public int EmployeeTypeId { get; set; }
        public string EmployeeTypeName { get; set; } = string.Empty;
    }

    public class EmployeeDto
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public int EmployeeTypeId { get; set; }
        public string? EmployeeTypeName { get; set; }
        public string? ImagePath { get; set; }
    }
}
