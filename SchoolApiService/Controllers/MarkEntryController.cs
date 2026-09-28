using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;
using SchoolApp.Models.DataModels;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MarkEntryController : ControllerBase
    {
        private readonly IMarkEntryService _markEntryService;

        public MarkEntryController(IMarkEntryService markEntryService)
        {
            _markEntryService = markEntryService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<MarkEntry>>>> GetMarkEntries()
        {
            var entries = await _markEntryService.GetAllMarkEntriesAsync();
            return Ok(ApiResponse<IEnumerable<MarkEntry>>.SuccessResponse(entries, "Mark entries retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<MarkEntry>>> GetMarkEntry(int id)
        {
            var markEntry = await _markEntryService.GetMarkEntryByIdAsync(id);
            if (markEntry == null)
            {
                return NotFound(ApiResponse<MarkEntry>.ErrorResponse($"No mark entry found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<MarkEntry>.SuccessResponse(markEntry, "Mark entry retrieved successfully."));
        }

        [HttpPost("GetStudents")]
        public async Task<ActionResult<ApiResponse<object>>> GetStudents([FromBody] MarkEntry markEntry)
        {
            var details = await _markEntryService.PopulateStudentMarksDetailsAsync(markEntry);
            return Ok(ApiResponse<object>.SuccessResponse(details, "Student marks details populated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<MarkEntry>>> CreateMarks([FromBody] MarkEntry markEntry)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MarkEntry>.ErrorResponse("Validation failed.", new List<string> { "Provided MarkEntry model is invalid." }, 400));
            }

            var (succeeded, errorMessage, statusCode, createdEntry) = await _markEntryService.CreateMarkEntryAsync(markEntry);
            if (!succeeded || createdEntry == null)
            {
                return StatusCode(statusCode, ApiResponse<MarkEntry>.ErrorResponse(errorMessage ?? "Failed to create mark entry.", statusCode: statusCode));
            }

            return Ok(ApiResponse<MarkEntry>.SuccessResponse(createdEntry, "Mark entry created successfully.", 201));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> UpdateMarks(int id, [FromBody] MarkEntry markEntry)
        {
            if (id != markEntry.MarkEntryId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload MarkEntryId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{markEntry.MarkEntryId}'." }, 400));
            }

            var (succeeded, errorMessage, updatedEntry) = await _markEntryService.UpdateMarkEntryAsync(markEntry);
            if (!succeeded)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse(errorMessage ?? "Failed to update mark entry.", statusCode: 400));
            }

            return Ok(ApiResponse<object>.SuccessResponse(updatedEntry!, "Mark entry updated successfully."));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteMarkEntry(int id)
        {
            var succeeded = await _markEntryService.DeleteMarkEntryAsync(id);
            if (!succeeded)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No mark entry found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Mark entry deleted successfully."));
        }
    }
}
