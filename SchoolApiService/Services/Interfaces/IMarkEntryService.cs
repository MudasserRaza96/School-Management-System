using SchoolApp.Models.DataModels;

namespace SchoolApiService.Services.Interfaces
{
    public interface IMarkEntryService
    {
        Task<IEnumerable<MarkEntry>> GetAllMarkEntriesAsync();
        Task<MarkEntry?> GetMarkEntryByIdAsync(int id);
        Task<IEnumerable<StudentMarksDetails>> PopulateStudentMarksDetailsAsync(MarkEntry markEntry);
        Task<(bool Succeeded, string? ErrorMessage, int StatusCode, MarkEntry? CreatedEntry)> CreateMarkEntryAsync(MarkEntry markEntry);
        Task<(bool Succeeded, string? ErrorMessage, MarkEntry? UpdatedEntry)> UpdateMarkEntryAsync(MarkEntry markEntry);
        Task<bool> DeleteMarkEntryAsync(int id);
        Task<bool> MarkEntryExistsAsync(int id);
    }
}
