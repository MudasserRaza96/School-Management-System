namespace SchoolApiService.DTOs
{
    public class StudentCreateDto
    {
        public int? AdmissionNo { get; set; }
        public int? EnrollmentNo { get; set; }
        public int UniqueStudentAttendanceNumber { get; set; }

        public string StudentName { get; set; } = string.Empty;
        public DateTime StudentDOB { get; set; }
        public string? StudentGender { get; set; }
        public string? StudentReligion { get; set; }
        public string? StudentBloodGroup { get; set; }
        public string? StudentNationality { get; set; }
        public string? StudentNIDNumber { get; set; }

        public string? StudentContactNumber1 { get; set; }
        public string? StudentContactNumber2 { get; set; }
        public string StudentEmail { get; set; } = string.Empty;

        public int StandardId { get; set; }
        public string? ImagePath { get; set; }

        public string? FatherName { get; set; }
        public string? FatherNID { get; set; }
        public string? FatherContactNumber { get; set; }

        public string? MotherName { get; set; }
        public string? MotherNID { get; set; }
        public string? MotherContactNumber { get; set; }

        public string? LocalGuardianName { get; set; }
        public string? LocalGuardianContactNumber { get; set; }

        public string? PermanentAddress { get; set; }
        public string? TemporaryAddress { get; set; }
    }

    public class StudentUpdateDto : StudentCreateDto
    {
        public int StudentId { get; set; }
    }

    public class StudentResponseDto : StudentCreateDto
    {
        public int StudentId { get; set; }
        public string? StandardName { get; set; }
    }
}
