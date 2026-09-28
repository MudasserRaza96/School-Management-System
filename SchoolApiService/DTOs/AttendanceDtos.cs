namespace SchoolApiService.DTOs
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public bool IsPresent { get; set; }
        public string? Remarks { get; set; }
    }

    public class StudentAttendanceDto
    {
        public int StudentAttendanceId { get; set; }
        public DateTime AttendanceDate { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class StaffAttendanceDto
    {
        public int StaffAttendanceId { get; set; }
        public DateTime Date { get; set; }
        public int StaffId { get; set; }
        public string? StaffName { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
