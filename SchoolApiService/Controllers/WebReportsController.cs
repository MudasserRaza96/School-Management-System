using Microsoft.AspNetCore.Mvc;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Controllers
{
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
        public ActionResult<string?> Get()
        {
            try
            {
                var pdfBase64 = _webReportService.GenerateReportPdfBase64();
                return Ok(pdfBase64);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}
