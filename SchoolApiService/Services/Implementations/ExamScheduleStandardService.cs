using Microsoft.EntityFrameworkCore;
using SchoolApiService.Services.Interfaces;
using SchoolApiService.ViewModels;
using SchoolApp.DAL.SchoolContext;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Implementations
{
    public class ExamScheduleStandardService : IExamScheduleStandardService
    {
        private readonly SchoolDbContext _context;

        public ExamScheduleStandardService(SchoolDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamScheduleStandardVM>> GetAllExamScheduleStandardsAsync()
        {
            return await _context.dbsExamScheduleStandard
                .Select(it => new ExamScheduleStandardVM
                {
                    ExamScheduleName = it.ExamSchedule.ExamScheduleName,
                    ExamScheduleStandardId = it.ExamScheduleStandardId,
                    ExamScheduleId = it.ExamScheduleId,
                    StandardId = it.StandardId,
                    StandardName = it.Standard.StandardName,
                    ExamSubjects = it.ExamSubjects.Select(es => new ExamSubjectVM
                    {
                        ExamTypeId = es.ExamTypeId,
                        SubjectId = es.SubjectId,
                        ExamDate = es.ExamDate,
                        ExamEndTime = es.ExamEndTime,
                        ExamStartTime = es.ExamStartTime,
                        ExamTypeName = es.ExamType.ExamTypeName,
                        SubjectCode = es.Subject.SubjectCode,
                        SubjectName = es.Subject.SubjectName,
                    })
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ExamScheduleStandardVM?> GetExamScheduleStandardByIdAsync(int id)
        {
            return await _context.dbsExamScheduleStandard
                .Select(it => new ExamScheduleStandardVM
                {
                    ExamScheduleName = it.ExamSchedule.ExamScheduleName,
                    ExamScheduleStandardId = it.ExamScheduleStandardId,
                    ExamScheduleId = it.ExamScheduleId,
                    StandardId = it.StandardId,
                    StandardName = it.Standard.StandardName,
                    ExamSubjects = it.ExamSubjects.Select(es => new ExamSubjectVM
                    {
                        ExamTypeId = es.ExamTypeId,
                        SubjectId = es.SubjectId,
                        ExamDate = es.ExamDate,
                        ExamEndTime = es.ExamEndTime,
                        ExamStartTime = es.ExamStartTime,
                        ExamTypeName = es.ExamType.ExamTypeName,
                        SubjectCode = es.Subject.SubjectCode,
                        SubjectName = es.Subject.SubjectName,
                    })
                })
                .AsNoTracking()
                .FirstOrDefaultAsync(es => es.ExamScheduleStandardId == id);
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> UpdateExamScheduleStandardAsync(int id, UpdateExamScheduleStandardVM request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var existingExamScheduleStandard = await _context.dbsExamScheduleStandard
                        .Include(es => es.ExamSubjects)
                        .FirstOrDefaultAsync(es => es.ExamScheduleStandardId == id);

                    if (existingExamScheduleStandard == null)
                    {
                        return (false, "Exam schedule standard Id not found.");
                    }

                    if (existingExamScheduleStandard.ExamScheduleId != request.ExamScheduleId)
                    {
                        existingExamScheduleStandard.ExamScheduleId = request.ExamScheduleId;
                    }

                    if (existingExamScheduleStandard.StandardId != request.StandardId)
                    {
                        existingExamScheduleStandard.StandardId = request.StandardId;
                        _context.dbsExamSubject.RemoveRange(existingExamScheduleStandard.ExamSubjects);
                    }

                    if (request.ExamSubjects != null)
                    {
                        foreach (var examSubject in request.ExamSubjects)
                        {
                            if (existingExamScheduleStandard.StandardId == await _context.dbsSubject
                                .Where(s => s.SubjectId == examSubject.SubjectId)
                                .Select(s => s.StandardId)
                                .SingleOrDefaultAsync())
                            {
                                DateTime.TryParse(examSubject.ExamStartTime, out DateTime startTime);
                                DateTime.TryParse(examSubject.ExamEndTime, out DateTime endTime);

                                var existingExamSubject = existingExamScheduleStandard.ExamSubjects.FirstOrDefault(es => es.SubjectId == examSubject.SubjectId);

                                if (existingExamSubject != null)
                                {
                                    existingExamSubject.ExamDate = examSubject.ExamDate;
                                    existingExamSubject.ExamStartTime = startTime;
                                    existingExamSubject.ExamEndTime = endTime;
                                    existingExamSubject.ExamTypeId = examSubject.ExamTypeId;
                                }
                                else
                                {
                                    existingExamScheduleStandard.ExamSubjects.Add(new ExamSubject
                                    {
                                        ExamDate = examSubject.ExamDate,
                                        ExamStartTime = startTime,
                                        ExamEndTime = endTime,
                                        SubjectId = examSubject.SubjectId,
                                        ExamScheduleStandardId = existingExamScheduleStandard.ExamScheduleStandardId,
                                        ExamTypeId = examSubject.ExamTypeId,
                                    });
                                }
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                    await transaction.CommitAsync();
                    return (true, null);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<(bool Succeeded, string? ErrorMessage)> CreateExamScheduleStandardAsync(CreateExamScheduleStandardVM request)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    if (await _context.dbsExamScheduleStandard.AnyAsync(it =>
                        it.ExamScheduleId == request.ExamScheduleId &&
                        it.StandardId == request.StandardId))
                    {
                        return (false, "Exam schedule standard already exist.");
                    }

                    var examScheduleStandard = new ExamScheduleStandard
                    {
                        ExamScheduleId = request.ExamScheduleId,
                        StandardId = request.StandardId
                    };

                    await _context.dbsExamScheduleStandard.AddAsync(examScheduleStandard);
                    await _context.SaveChangesAsync();

                    List<ExamSubject> examSubjects = new List<ExamSubject>();
                    if (request.ExamSubjects != null)
                    {
                        foreach (var examSubject in request.ExamSubjects)
                        {
                            if (request.StandardId == await _context.dbsSubject.Where(it => it.SubjectId == examSubject.SubjectId).Select(it => it.StandardId).SingleOrDefaultAsync())
                            {
                                DateTime.TryParse(examSubject.ExamStartTime, out DateTime startTime);
                                DateTime.TryParse(examSubject.ExamEndTime, out DateTime endTime);

                                examSubjects.Add(new ExamSubject
                                {
                                    ExamDate = examSubject.ExamDate,
                                    ExamStartTime = startTime,
                                    ExamEndTime = endTime,
                                    SubjectId = examSubject.SubjectId,
                                    ExamScheduleStandardId = examScheduleStandard.ExamScheduleStandardId,
                                    ExamTypeId = examSubject.ExamTypeId,
                                });
                            }
                        }
                    }

                    if (examSubjects.Count > 0)
                    {
                        await _context.dbsExamSubject.AddRangeAsync(examSubjects);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return (true, null);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<bool> DeleteExamScheduleStandardAsync(int id)
        {
            var examScheduleStandard = await _context.dbsExamScheduleStandard
                .Include(e => e.ExamSubjects)
                .FirstOrDefaultAsync(e => e.ExamScheduleStandardId == id);

            if (examScheduleStandard == null)
            {
                return false;
            }

            _context.dbsExamScheduleStandard.Remove(examScheduleStandard);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExamScheduleStandardExistsAsync(int id)
        {
            return await _context.dbsExamScheduleStandard.AnyAsync(e => e.ExamScheduleStandardId == id);
        }
    }
}
