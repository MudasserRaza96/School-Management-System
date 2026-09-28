using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Models;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class WebReportsController : ControllerBase
    {
        private readonly IWebReportService _webReportService;

        public WebReportsController(IWebReportService webReportService)
        {
            _webReportService = webReportService;
        }

        [HttpGet]
        public ActionResult<ApiResponse<string>> Get()
        {
            var pdfBase64 = _webReportService.GenerateReportPdfBase64();
            if (string.IsNullOrEmpty(pdfBase64))
            {
                return BadRequest(ApiResponse<string>.ErrorResponse(
                    "Report generation failed.",
                    new List<string> { "Failed to generate report PDF base64 output." },
                    400
                ));
            }
            return Ok(ApiResponse<string>.SuccessResponse(pdfBase64, "Web report generated successfully."));
        }
    }
}
