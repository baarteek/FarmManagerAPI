using FarmManagerAPI.DTOs;

namespace FarmManagerAPI.Services.Interfaces
{
    public interface IReportService
    {
        Task<List<AgrotechnicalActivitiesDTO>> GetAgrotechnicalActivitiesReportData(Guid farmId);
        string GenerateAgrotechnicalActivitiesReportHtml(List<AgrotechnicalActivitiesDTO> agrtechnicalActivities);
        public byte[] GenerateAgrotechnicalActivitiesReportPdf(string htmlContent);
    }
}
