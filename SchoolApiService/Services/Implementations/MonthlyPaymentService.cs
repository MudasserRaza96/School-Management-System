using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class MonthlyPaymentService : IMonthlyPaymentService
    {
        private readonly SchoolDbContext _context;

        public MonthlyPaymentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MonthlyPayment>> GetAllAsync()
        {
            return await _context.monthlyPayments
                .Include(fp => fp.PaymentDetails)
                .Include(fp => fp.paymentMonths)
                .Include(fp => fp.Student)
                .Include(fp => fp.dueBalances)
                .ToListAsync();
        }

        public async Task<MonthlyPayment?> GetByIdAsync(int id)
        {
            return await _context.monthlyPayments
                .Include(fp => fp.PaymentDetails)
                .Include(fp => fp.paymentMonths)
                .Include(fp => fp.Student)
                .Include(fp => fp.dueBalances)
                .FirstOrDefaultAsync(fp => fp.MonthlyPaymentId == id);
        }

        public async Task<MonthlyPayment> CreateAsync(MonthlyPayment monthlyPayment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await AttachFeeAsync(monthlyPayment);
                await AttachAcademicMonthAsync(monthlyPayment);
                await CalculatePaymentFieldsAsync(monthlyPayment);

                _context.monthlyPayments.Add(monthlyPayment);
                await _context.SaveChangesAsync();

                UpdateDueBalance(monthlyPayment);
                await SavePaymentDetailAsync(monthlyPayment);
                await SaveMonthDetailsAsync(monthlyPayment);

                await transaction.CommitAsync();
                return monthlyPayment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<MonthlyPayment?> UpdateAsync(int id, MonthlyPayment updatedmonthlyPayment)
        {
            if (id != updatedmonthlyPayment.MonthlyPaymentId)
            {
                throw new ArgumentException("ID Mismatch");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingMonthlyPayment = await _context.monthlyPayments
                    .Include(p => p.fees)
                    .Include(p => p.paymentMonths)
                    .Include(p => p.dueBalances)
                    .Include(p => p.PaymentDetails)
                    .FirstOrDefaultAsync(p => p.MonthlyPaymentId == id);

                if (existingMonthlyPayment == null)
                {
                    return null;
                }

                existingMonthlyPayment.dueBalances.Clear();
                existingMonthlyPayment.StudentId = updatedmonthlyPayment.StudentId;
                existingMonthlyPayment.TotalFeeAmount = updatedmonthlyPayment.TotalFeeAmount;
                existingMonthlyPayment.Waver = updatedmonthlyPayment.Waver;
                existingMonthlyPayment.PreviousDue = updatedmonthlyPayment.PreviousDue;
                existingMonthlyPayment.AmountPaid = updatedmonthlyPayment.AmountPaid;

                existingMonthlyPayment.paymentMonths.Clear();
                existingMonthlyPayment.PaymentDetails.Clear();
                existingMonthlyPayment.dueBalances.Clear();

                await AttachFeeAsync(existingMonthlyPayment, updatedmonthlyPayment);
                await AttachAcademicMonthAsync(existingMonthlyPayment, updatedmonthlyPayment);

                await CalculatePaymentFieldsAsync2(existingMonthlyPayment);
                UpdateDueBalance(existingMonthlyPayment);

                await _context.SaveChangesAsync();

                await SaveMonthDetailsAsync(existingMonthlyPayment);
                await SavePaymentDetailAsync(existingMonthlyPayment);

                await transaction.CommitAsync();
                return existingMonthlyPayment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var monthlyPayment = await _context.monthlyPayments
                .Include(fp => fp.fees)
                .Include(fp => fp.academicMonths)
                .FirstOrDefaultAsync(fp => fp.MonthlyPaymentId == id);

            if (monthlyPayment == null)
            {
                return false;
            }

            foreach (var academicMonth in monthlyPayment.academicMonths)
            {
                academicMonth.monthlyPayment = null;
            }

            _context.monthlyPayments.Remove(monthlyPayment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.monthlyPayments.AnyAsync(e => e.MonthlyPaymentId == id);
        }

        private async Task CalculatePaymentFieldsAsync(MonthlyPayment monthlyPayment)
        {
            var student = await _context.dbsStudent
                .Where(s => s.StudentId == monthlyPayment.StudentId)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                throw new Exception("Invalid Student Id: " + monthlyPayment.StudentId);
            }
            var studentId = monthlyPayment.StudentId;
            var academicMonthsCount = monthlyPayment.academicMonths?.Count ?? 0;

            monthlyPayment.TotalFeeAmount = monthlyPayment.fees?.Sum(fs => fs.Amount * academicMonthsCount) ?? 0;

            var previousDue = await _context.dbsDueBalance
                .Where(b => b.StudentId == studentId)
                .Select(b => b.DueBalanceAmount)
                .FirstOrDefaultAsync();

            monthlyPayment.PreviousDue = previousDue ?? 0;
            monthlyPayment.TotalAmount = monthlyPayment.TotalFeeAmount - (monthlyPayment.TotalFeeAmount * (monthlyPayment.Waver / 100)) + monthlyPayment.PreviousDue;
            monthlyPayment.AmountRemaining = monthlyPayment.TotalAmount - monthlyPayment.AmountPaid;
        }

        private async Task CalculatePaymentFieldsAsync2(MonthlyPayment monthlyPayment)
        {
            var student = await _context.dbsStudent
                .Where(s => s.StudentId == monthlyPayment.StudentId)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                throw new Exception("Invalid Student Id: " + monthlyPayment.StudentId);
            }

            var academicMonthsCount = monthlyPayment.academicMonths?.Count ?? 0;
            monthlyPayment.TotalFeeAmount = monthlyPayment.fees?.Sum(fs => fs.Amount * academicMonthsCount) ?? 0;

            monthlyPayment.TotalAmount = monthlyPayment.TotalFeeAmount - (monthlyPayment.TotalFeeAmount * (monthlyPayment.Waver / 100)) + monthlyPayment.PreviousDue;
            monthlyPayment.AmountRemaining = monthlyPayment.TotalAmount - monthlyPayment.AmountPaid;
        }

        private async Task AttachAcademicMonthAsync(MonthlyPayment existingMonthlyPayment, MonthlyPayment updatedMonthlyPayment)
        {
            if (updatedMonthlyPayment.academicMonths != null && updatedMonthlyPayment.academicMonths.Any())
            {
                existingMonthlyPayment.academicMonths = await _context.dbsAcademicMonths
                    .Where(am => updatedMonthlyPayment.academicMonths.Select(m => m.MonthId).Contains(am.MonthId))
                    .ToListAsync();
            }
        }

        private async Task AttachFeeAsync(MonthlyPayment existingMonthlyPayment, MonthlyPayment updatedMonthlyPayment)
        {
            if (updatedMonthlyPayment.fees != null && updatedMonthlyPayment.fees.Any())
            {
                existingMonthlyPayment.fees = await _context.fees
                    .Where(am => updatedMonthlyPayment.fees.Select(m => m.FeeId).Contains(am.FeeId))
                    .ToListAsync();
            }
        }

        private async Task AttachFeeAsync(MonthlyPayment monthlyPayment)
        {
            if (monthlyPayment.fees != null && monthlyPayment.fees.Any())
            {
                monthlyPayment.fees = await _context.fees
                    .Where(fs => monthlyPayment.fees.Select(f => f.FeeId).Contains(fs.FeeId))
                    .ToListAsync();
            }
        }

        private async Task AttachAcademicMonthAsync(MonthlyPayment monthlyPayment)
        {
            if (monthlyPayment.academicMonths != null && monthlyPayment.academicMonths.Any())
            {
                monthlyPayment.academicMonths = await _context.dbsAcademicMonths
                    .Where(am => monthlyPayment.academicMonths.Select(m => m.MonthId).Contains(am.MonthId))
                    .ToListAsync();
            }
        }

        private async Task SaveMonthDetailsAsync(MonthlyPayment monthlyPayment)
        {
            if (monthlyPayment.academicMonths != null && monthlyPayment.academicMonths.Any())
            {
                foreach (var academicMonth in monthlyPayment.academicMonths)
                {
                    var paymentMonth = new PaymentMonth
                    {
                        MonthlyPaymentId = monthlyPayment.MonthlyPaymentId,
                        MonthName = academicMonth.MonthName
                    };

                    _context.paymentMonths.Add(paymentMonth);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task SavePaymentDetailAsync(MonthlyPayment monthlyPayment)
        {
            if (monthlyPayment.fees != null && monthlyPayment.fees.Any())
            {
                foreach (var fees in monthlyPayment.fees)
                {
                    var feeType = _context.dbsFeeType
                        .Where(ft => ft.FeeTypeId == fees.FeeTypeId)
                        .FirstOrDefault();

                    var paymentDetails = new PaymentDetail
                    {
                        MonthlyPaymentId = monthlyPayment.MonthlyPaymentId,
                        FeeAmount = fees.Amount,
                        FeeName = feeType?.TypeName
                    };

                    _context.PaymentDetails.Add(paymentDetails);
                }

                await _context.SaveChangesAsync();
            }
        }

        private void UpdateDueBalance(MonthlyPayment monthlyPayment)
        {
            var dueBalance = _context.dbsDueBalance
                .Where(db => db.StudentId == monthlyPayment.StudentId)
                .FirstOrDefault();

            if (dueBalance != null)
            {
                dueBalance.DueBalanceAmount = monthlyPayment.AmountRemaining;
                dueBalance.LastUpdate = DateTime.Now;
            }
            else
            {
                _context.dbsDueBalance.Add(new DueBalance
                {
                    StudentId = monthlyPayment.StudentId,
                    DueBalanceAmount = monthlyPayment.AmountRemaining,
                    LastUpdate = DateTime.Now
                });
            }

            _context.SaveChanges();
        }
    }
}
