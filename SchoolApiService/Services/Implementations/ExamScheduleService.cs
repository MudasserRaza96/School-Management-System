using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;
using static SchoolApiService.Controllers.ExamSchedulesController;

namespace SchoolApiService.Services.Implementations
{
    public class ExamScheduleService : IExamScheduleService
    {
        private readonly SchoolDbContext _context;

        public ExamScheduleService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamScheduleVM>> GetAllExamSchedulesAsync()
        {
            return await _context.dbsExamSchedule
                .Select(it => new ExamScheduleVM
                {
                    ExamScheduleId = it.ExamScheduleId,
                    ExamScheduleName = it.ExamScheduleName,
                    ExamScheduleStandards = it.ExamScheduleStandards.Select(ess => new ExamScheduleStandardForExamScheduleVM
                    {
                        StandardName = ess.Standard.StandardName,
                        ExamSubjects = ess.ExamSubjects.Select(es => new ExamSubjectVM
                        {
                            ExamStartTime = es.ExamStartTime,
                            ExamEndTime = es.ExamEndTime,
                            ExamDate = es.ExamDate,
                            ExamTypeName = es.ExamType.ExamTypeName,
                            SubjectName = es.Subject.SubjectName,
                            SubjectCode = es.Subject.SubjectCode
                        })
                    })
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<GetExamScheduleOptionsResponse>> GetExamScheduleOptionsAsync()
        {
            return await _context.dbsExamSchedule
                .Select(it => new GetExamScheduleOptionsResponse(it.ExamScheduleId, it.ExamScheduleName))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ExamScheduleVM?> GetExamScheduleByIdAsync(int id)
        {
            return await _context.dbsExamSchedule
                .Select(it => new ExamScheduleVM
                {
                    ExamScheduleId = it.ExamScheduleId,
                    ExamScheduleName = it.ExamScheduleName,
                    ExamScheduleStandards = it.ExamScheduleStandards.Select(ess => new ExamScheduleStandardForExamScheduleVM
                    {
                        StandardName = ess.Standard.StandardName,
                        ExamSubjects = ess.ExamSubjects.Select(es => new ExamSubjectVM
                        {
                            ExamStartTime = es.ExamStartTime,
                            ExamEndTime = es.ExamEndTime,
                            ExamDate = es.ExamDate,
                            ExamTypeName = es.ExamType.ExamTypeName,
                            SubjectName = es.Subject.SubjectName,
                            SubjectCode = es.Subject.SubjectCode
                        })
                    })
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(it => it.ExamScheduleId == id);
        }

        public async Task<ExamSchedule> CreateExamScheduleAsync(ExamSchedule examSchedule)
        {
            _context.dbsExamSchedule.Add(examSchedule);
            await _context.SaveChangesAsync();
            return examSchedule;
        }

        public async Task<(bool Succeeded, bool ConcurrencyError)> UpdateExamScheduleAsync(int id, ExamSchedule examSchedule)
        {
            if (id != examSchedule.ExamScheduleId)
            {
                return (false, false);
            }

            _context.Entry(examSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return (true, false);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ExamScheduleExistsAsync(id))
                {
                    return (false, true);
                }
                throw;
            }
        }

        public async Task<bool> DeleteExamScheduleAsync(int id)
        {
            var examSchedule = await _context.dbsExamSchedule.FindAsync(id);
            if (examSchedule == null)
            {
                return false;
            }

            var relatedExamScheduleStandards = await _context.dbsExamScheduleStandard
                .Where(es => es.ExamScheduleId == id)
                .ToListAsync();

            foreach (var examScheduleStandard in relatedExamScheduleStandards)
            {
                examScheduleStandard.ExamScheduleId = null;
                _context.Entry(examScheduleStandard).State = EntityState.Modified;
            }

            await _context.SaveChangesAsync();

            _context.dbsExamSchedule.Remove(examSchedule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExamScheduleExistsAsync(int id)
        {
            return await _context.dbsExamSchedule.AnyAsync(e => e.ExamScheduleId == id);
        }
    }
}
