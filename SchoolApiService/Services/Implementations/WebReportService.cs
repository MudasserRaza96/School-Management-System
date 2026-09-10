using FastReport.Data;
using FastReport.Export.PdfSimple;
using FastReport.Web;
using SchoolApiService.Services.Interfaces;

namespace SchoolApiService.Services.Implementations
{
    public class WebReportService : IWebReportService
    {
        private readonly IWebHostEnvironment _webHost;

        public WebReportService(IWebHostEnvironment webHost)
        {
            _webHost = webHost;
        }

        public string? GenerateReportPdfBase64()
        {
            WebReport webReport = new WebReport();
            webReport.Report.Load(Path.Combine(_webHost.ContentRootPath, "Reports", "Untitled.frx"));

            MsSqlDataConnection sqlConnection = new MsSqlDataConnection
            {
                ConnectionString = "Server=(localdb)\\mssqllocaldb; Database=SchoolSystemDb; Trusted_Connection=True;"
            };

            webReport.Report.SetParameterValue("dbCon", sqlConnection.ConnectionString);
            webReport.Report.Prepare();

            PDFSimpleExport export = new PDFSimpleExport();
            using MemoryStream ms = new MemoryStream();
            webReport.Report.Export(export, ms);
            ms.Position = 0;
            byte[] pdfBytes = ms.ToArray();

            return Convert.ToBase64String(pdfBytes);
        }
    }
}
