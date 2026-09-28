namespace SchoolApiService.DTOs
{
    public class ExamTypeDto
    {
        public int ExamTypeId { get; set; }
        public string ExamTypeName { get; set; } = string.Empty;
    }

    public class ExamScheduleDto
    {
        public int ExamScheduleId { get; set; }
        public int ExamTypeId { get; set; }
        public string? ExamTypeName { get; set; }
        public DateTime ExamDate { get; set; }
    }

    public class ExamScheduleStandardDto
    {
        public int ExamScheduleStandardId { get; set; }
        public int ExamScheduleId { get; set; }
        public int StandardId { get; set; }
        public string? StandardName { get; set; }
    }

    public class ExamSubjectDto
    {
        public int ExamSubjectId { get; set; }
        public int ExamScheduleId { get; set; }
        public int SubjectId { get; set; }
        public string? SubjectName { get; set; }
        public decimal TotalMarks { get; set; }
        public decimal PassMarks { get; set; }
    }

    public class MarkDto
    {
        public int MarkId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public int SubjectId { get; set; }
        public decimal ObtainedMarks { get; set; }
        public string? Grade { get; set; }
    }

    public class MarkEntryDto
    {
        public int MarkEntryId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public int ExamScheduleId { get; set; }
        public decimal TotalObtainedMarks { get; set; }
        public string? FinalGrade { get; set; }
    }
}
