using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace FarmManagerAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("html/{farmId}")]
        public async Task<ActionResult<string>> GetAgrotechnicalActivitiesReportHtml(Guid farmId)
        {
            try
            {
                var agrotechnicalActivities = await _reportService.GetAgrotechnicalActivitiesReportData(farmId);
                var htmlReport = _reportService.GenerateAgrotechnicalActivitiesReportHtml(agrotechnicalActivities);
                return Ok(htmlReport);
            } catch (Exception ex) 
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("pdf/{farmId}")]
        [Produces("application/pdf")]
        public async Task<ActionResult> GetAgrotechnicalActivitiesReportPdf(Guid farmId)
        {
            try
            {
                var agrotechnicalActivities = await _reportService.GetAgrotechnicalActivitiesReportData(farmId);
                var htmlReport = _reportService.GenerateAgrotechnicalActivitiesReportHtml(agrotechnicalActivities);
                var pdfReport = _reportService.GenerateAgrotechnicalActivitiesReportPdf(htmlReport);
                var pdfName = "Report " + DateTime.Now.ToString("g") + ".pdf";
                return File(pdfReport, "application/pdf", pdfName);
            } catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("excel/{farmId}")]
        [Produces("application/vnd.ms-excel")]
        public async Task<IActionResult> GetAgrotechnicalActivitiesReportExcelFromHtml(Guid farmId)
        {
            try
            {
                var agrotechnicalActivities = await _reportService.GetAgrotechnicalActivitiesReportData(farmId);
                var htmlContent = _reportService.GenerateAgrotechnicalActivitiesReportHtml(agrotechnicalActivities);
                var contentBytes = Encoding.UTF8.GetBytes(htmlContent);
                var fileName = "Report " + DateTime.Now.ToString("g") + ".xls";

                return File(contentBytes, "application/vnd.ms-excel", fileName);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
