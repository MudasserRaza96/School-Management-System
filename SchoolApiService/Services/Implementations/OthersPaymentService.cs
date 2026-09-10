using Microsoft.EntityFrameworkCore;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class OthersPaymentService : IOthersPaymentService
    {
        private readonly SchoolDbContext _context;

        public OthersPaymentService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OthersPayment>> GetAllAsync()
        {
            return await _context.othersPayments
                .Include(fp => fp.otherPaymentDetails)
                .Include(fp => fp.Student)
                .ToListAsync();
        }

        public async Task<OthersPayment?> GetByIdAsync(int id)
        {
            return await _context.othersPayments
                .Include(fp => fp.otherPaymentDetails)
                .Include(fp => fp.Student)
                .FirstOrDefaultAsync(fp => fp.OthersPaymentId == id);
        }

        public async Task<OthersPayment> CreateAsync(OthersPayment othersPayment)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                await AttachFeeAsync(othersPayment);
                await CalculatePaymentFieldsAsync(othersPayment);

                _context.othersPayments.Add(othersPayment);
                await _context.SaveChangesAsync();

                UpdateDueBalance(othersPayment);
                await SavePaymentDetailAsync(othersPayment);

                await transaction.CommitAsync();
                return othersPayment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OthersPayment?> UpdateAsync(int id, OthersPayment updatedPayment)
        {
            if (id != updatedPayment.OthersPaymentId)
            {
                throw new ArgumentException("Invalid ID");
            }

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var existingPayment = await _context.othersPayments
                    .Include(fp => fp.fees)
                    .Include(fp => fp.otherPaymentDetails)
                    .FirstOrDefaultAsync(p => p.OthersPaymentId == id);

                if (existingPayment == null)
                {
                    return null;
                }

                existingPayment.StudentId = updatedPayment.StudentId;
                existingPayment.TotalAmount = updatedPayment.TotalAmount;
                existingPayment.AmountPaid = updatedPayment.AmountPaid;

                existingPayment.otherPaymentDetails.Clear();

                await AttachFeeAsync(existingPayment, updatedPayment);
                await SavePaymentDetailAsync(existingPayment);
                await CalculatePaymentFieldsAsync(existingPayment);

                await _context.SaveChangesAsync();
                UpdateDueBalance(existingPayment);

                await transaction.CommitAsync();
                return existingPayment;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var othersPayment = await _context.othersPayments
                .Include(fp => fp.fees)
                .FirstOrDefaultAsync(fp => fp.OthersPaymentId == id);

            if (othersPayment == null)
            {
                return false;
            }

            _context.othersPayments.Remove(othersPayment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.othersPayments.AnyAsync(e => e.OthersPaymentId == id);
        }

        private async Task AttachFeeAsync(OthersPayment existingOthersPayment, OthersPayment updatedOthersPayment)
        {
            if (updatedOthersPayment.fees != null && updatedOthersPayment.fees.Any())
            {
                existingOthersPayment.fees = await _context.fees
                    .Where(am => updatedOthersPayment.fees.Select(m => m.FeeId).Contains(am.FeeId))
                    .ToListAsync();
            }
        }

        private async Task AttachFeeAsync(OthersPayment othersPayment)
        {
            if (othersPayment.fees != null && othersPayment.fees.Any())
            {
                othersPayment.fees = await _context.fees
                    .Where(fs => othersPayment.fees.Select(f => f.FeeId).Contains(fs.FeeId))
                    .ToListAsync();
            }
        }

        private void UpdateDueBalance(OthersPayment othersPayment)
        {
            var dueBalance = _context.dbsDueBalance
                .Where(db => db.StudentId == othersPayment.StudentId)
                .FirstOrDefault();

            if (dueBalance != null)
            {
                dueBalance.DueBalanceAmount += othersPayment.AmountRemaining;
                dueBalance.LastUpdate = DateTime.Now;
            }
            else
            {
                _context.dbsDueBalance.Add(new DueBalance
                {
                    StudentId = othersPayment.StudentId,
                    DueBalanceAmount = othersPayment.AmountRemaining,
                    LastUpdate = DateTime.Now
                });
            }

            _context.SaveChanges();
        }

        private async Task SavePaymentDetailAsync(OthersPayment othersPayment)
        {
            if (othersPayment.fees != null && othersPayment.fees.Any())
            {
                foreach (var fees in othersPayment.fees)
                {
                    var feeType = _context.dbsFeeType
                        .Where(ft => ft.FeeTypeId == fees.FeeTypeId)
                        .FirstOrDefault();

                    var otherPaymentDetail = new OtherPaymentDetail
                    {
                        OthersPaymentId = othersPayment.OthersPaymentId,
                        FeeAmount = fees.Amount,
                        FeeName = feeType?.TypeName
                    };

                    _context.otherPaymentDetails.Add(otherPaymentDetail);
                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task CalculatePaymentFieldsAsync(OthersPayment othersPayment)
        {
            var student = await _context.dbsStudent
                .Where(s => s.StudentId == othersPayment.StudentId)
                .FirstOrDefaultAsync();

            if (student == null)
            {
                throw new Exception("Invalid Student Id: " + othersPayment.StudentId);
            }

            othersPayment.TotalAmount = othersPayment.fees?.Sum(fs => fs.Amount) ?? 0;
            othersPayment.AmountRemaining = othersPayment.TotalAmount - othersPayment.AmountPaid;
        }
    }
}
