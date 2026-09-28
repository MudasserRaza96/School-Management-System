using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.DTOs;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectsController : ControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectsController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<SubjectDto>>>> GetSubjects()
        {
            var subjs = await _subjectService.GetAllAsync();
            return Ok(ApiResponse<IEnumerable<SubjectDto>>.SuccessResponse(subjs, "Subjects retrieved successfully."));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<SubjectDto>>> GetSubject(int id)
        {
            var subj = await _subjectService.GetByIdAsync(id);
            if (subj == null)
            {
                return NotFound(ApiResponse<SubjectDto>.ErrorResponse($"No subject found with ID {id}.", statusCode: 404));
            }

            return Ok(ApiResponse<SubjectDto>.SuccessResponse(subj, "Subject retrieved successfully."));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> PutSubject(int id, SubjectDto dto)
        {
            if (id != dto.SubjectId)
            {
                return BadRequest(ApiResponse<object>.ErrorResponse("URL ID and payload SubjectId mismatch.", new List<string> { $"Provided ID '{id}' does not match payload ID '{dto.SubjectId}'." }, 400));
            }

            var success = await _subjectService.UpdateAsync(id, dto);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No subject found with ID {id} to update.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Subject updated successfully."));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<SubjectDto>>> PostSubject(SubjectDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.SubjectName))
            {
                return BadRequest(ApiResponse<SubjectDto>.ErrorResponse("Subject creation failed.", new List<string> { "SubjectName is required." }, 400));
            }

            var created = await _subjectService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetSubject), new { id = created.SubjectId }, ApiResponse<SubjectDto>.SuccessResponse(created, "Subject created successfully.", 201));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<object>>> DeleteSubject(int id)
        {
            var success = await _subjectService.DeleteAsync(id);
            if (!success)
            {
                return NotFound(ApiResponse<object>.ErrorResponse($"No subject found with ID {id} to delete.", statusCode: 404));
            }

            return Ok(ApiResponse<object>.SuccessResponse(null!, "Subject deleted successfully."));
        }
    }
}
