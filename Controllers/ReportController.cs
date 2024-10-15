using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("/AgrotechnicalActivitiesReport/{farmId}")]
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
    }
}
