namespace SchoolApiService.DTOs
{
    public class FeeDto
    {
        public int FeeId { get; set; }
        public string FeeName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }

    public class FeeTypeDto
    {
        public int FeeTypeId { get; set; }
        public string FeeTypeName { get; set; } = string.Empty;
    }

    public class FeeStructureDto
    {
        public int FeeStructureId { get; set; }
        public int StandardId { get; set; }
        public string? StandardName { get; set; }
        public int FeeTypeId { get; set; }
        public string? FeeTypeName { get; set; }
        public decimal Amount { get; set; }
    }

    public class FeePaymentDto
    {
        public int FeePaymentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public decimal AmountPaid { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class MonthlyPaymentDto
    {
        public int MonthlyPaymentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public int MonthId { get; set; }
        public string? MonthName { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class OthersPaymentDto
    {
        public int OthersPaymentId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }

    public class DueBalanceDto
    {
        public int DueBalanceId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public decimal DueAmount { get; set; }
    }
}
