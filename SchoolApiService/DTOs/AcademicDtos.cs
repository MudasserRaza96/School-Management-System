namespace SchoolApiService.DTOs
{
    public class AcademicMonthDto
    {
        public int MonthId { get; set; }
        public string MonthName { get; set; } = string.Empty;
    }

    public class DepartmentDto
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class StandardDto
    {
        public int StandardId { get; set; }
        public string StandardName { get; set; } = string.Empty;
    }

    public class SubjectDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; } = string.Empty;
        public int? SubjectCode { get; set; }
    }
}
