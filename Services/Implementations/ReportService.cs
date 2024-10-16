using FarmManagerAPI.DTOs;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FarmManagerAPI.Services.Implementations
{
    public class ReportService : IReportService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IReferenceParcelRepository _referenceParcelRepository;
        private readonly ICropRepository _cropRepository;
        private readonly ICultivationOperationRepository _operationRepository;
        private readonly IPlantProtectionRepository _plantProtectionRepository;
        private readonly IFertilizationRepository _fertilizationRepository;

        public ReportService(IFarmRepository farmRepository ,IFieldRepository fieldRepository, IReferenceParcelRepository referenceParcelRepository, 
                                ICropRepository cropRepository, ICultivationOperationRepository operationRepository,
                                IPlantProtectionRepository plantProtectionRepository, IFertilizationRepository fertilizationRepository)
        {
            _farmRepository = farmRepository;
            _fieldRepository = fieldRepository;
            _referenceParcelRepository = referenceParcelRepository;
            _cropRepository = cropRepository;
            _operationRepository = operationRepository;
            _plantProtectionRepository = plantProtectionRepository;
            _fertilizationRepository = fertilizationRepository;
        }

        public async Task<List<AgrotechnicalActivitiesDTO>> GetAgrotechnicalActivitiesReportData(Guid farmId)
        {
            var farm = await _farmRepository.GetById(farmId);
            if (farm == null)
            {
                throw new Exception("Farm not found");
            }

            var fields = await _fieldRepository.GetFieldsByFarmId(farm.Id);
            if(fields.IsNullOrEmpty())
            {
                throw new Exception("No fields for this farm.");
            }

            List<AgrotechnicalActivitiesDTO> agrotechnicalActivities = new List<AgrotechnicalActivitiesDTO>();
            foreach (var field in fields)
            {
                var referenceParcels = await _referenceParcelRepository.GetParcelsByFieldId(field.Id);
                var crop = await _cropRepository.GetActiveCropByFieldId(field.Id);
                if(crop != null)
                {
                    var cultivationOperations = await _operationRepository.GetCultivationOperationsByCropId(crop.Id);
                    foreach (var operation in cultivationOperations)
                    {
                        foreach(var parcel in referenceParcels)
                        {
                            agrotechnicalActivities.Add(new AgrotechnicalActivitiesDTO
                            {
                                CropIdentifier = crop.CropIdentifier,
                                PlotNumber = parcel.ParcelNumber,
                                Date = operation.Date,
                                Area = parcel.Area,
                                TypeOfUse = crop.Name,
                                TypeOfActivity = operation.Name,
                                NameOfPlantProtectionProduct = "-",
                                AmountOfPlatnProtectionProduct = "-",
                                PackageNumber = "", // TODO
                                Comments = operation.Description,
                            });
                        }
                    }

                    var plantProtections = await _plantProtectionRepository.GetPlantProtectionsByCropId(crop.Id);
                    foreach(var plantProtection in plantProtections)
                    {
                        foreach (var parcel in referenceParcels)
                        {
                            agrotechnicalActivities.Add(new AgrotechnicalActivitiesDTO
                            {
                                CropIdentifier= crop.CropIdentifier,
                                PlotNumber = parcel.ParcelNumber,
                                Date = plantProtection.Date,
                                Area = parcel.Area,
                                TypeOfUse = crop.Name,
                                TypeOfActivity = "oprysk",
                                NameOfPlantProtectionProduct = plantProtection.NameOfProduct,
                                AmountOfPlatnProtectionProduct = plantProtection.Quantity.ToString() + " l/ha",
                                PackageNumber = "", // TODO
                                Comments = plantProtection.Description,
                            });
                        }
                    }

                    var ferelizations = await _fertilizationRepository.GetFertilizationsByCropId(crop.Id);
                    foreach (var ferelization in ferelizations)
                    {
                        foreach(var parcel in referenceParcels)
                        {
                            agrotechnicalActivities.Add(new AgrotechnicalActivitiesDTO
                            {
                                CropIdentifier = crop.CropIdentifier,
                                PlotNumber = parcel.ParcelNumber,
                                Date = ferelization.Date,
                                Area = parcel.Area,
                                TypeOfUse = crop.Name,
                                TypeOfActivity = "nawożenie",
                                NameOfPlantProtectionProduct = ferelization.NameOfProduct,
                                AmountOfPlatnProtectionProduct = ferelization.Quantity.ToString() + " t/ha",
                                PackageNumber = "", // TODO
                                Comments = ferelization.Description
                            });
                        }
                    }
                }
            }
            return agrotechnicalActivities;
        }


        public string GenerateAgrotechnicalActivitiesReportHtml(List<AgrotechnicalActivitiesDTO> agrtechnicalActivities)
        {
            var htmlBuilder = new StringBuilder();

            htmlBuilder.Append("<html><head><style>");
            htmlBuilder.Append("<title>Report</title> <style> table { width: 100%; border-collapse: collapse; } th, td { border: 1px solid black; padding: 5px; text-align: center; vertical-align: middle; } </style> </head> <body>");
            htmlBuilder.Append("<table> <tr> <th colspan=\"10\">WYKAZ DZIAŁAŃ AGROTECHNICZNYCH</th> </tr> <tr> <th>Oznaczenie działki (literowe)</th> <th>Numer działki ewidencyjnej</th> <th>Data wykonania czynności [dd/mm/rrrr]</th> <th>Powierzchnia działki/uprawy [ha,a]</th> <th>Rodzaj użytkowania (uprawa w plonie głównym/uprawa w poplonie)</th> <th>Rodzaj wykonywanej czynności</th> <th>Nazwa środka ochrony roślin</th> <th>Zastosowana ilość środka ochrony roślin/nawozu</th> <th>Działanie/interwencja/praktyka Nummer pakietu lub wariantu</th> <th>Uwagi/powierzchnia wykonywanej czynności</th> </tr> <tr> <th>1</th> <th>2</th> <th>3</th> <th>4</th> <th>5</th> <th>6</th> <th>7</th> <th>8</th> <th>9</th> <th>10</th> </tr>");

            foreach(var aa in agrtechnicalActivities)
            {
                htmlBuilder.Append("<tr>");
                htmlBuilder.Append($"<td>{aa.CropIdentifier}</td>");
                htmlBuilder.Append($"<td>{aa.PlotNumber}</td>");
                htmlBuilder.Append($"<td>{aa.Date}</td>");
                htmlBuilder.Append($"<td>{aa.Area}</td>");
                htmlBuilder.Append($"<td>{aa.TypeOfUse}</td>");
                htmlBuilder.Append($"<td>{aa.TypeOfActivity}</td>");
                htmlBuilder.Append($"<td>{aa.NameOfPlantProtectionProduct}</td>");
                htmlBuilder.Append($"<td>{aa.AmountOfPlatnProtectionProduct}</td>");
                htmlBuilder.Append($"<td>{aa.PackageNumber}</td>");
                htmlBuilder.Append($"<td>{aa.Comments}</td>");
                htmlBuilder.Append("</tr>");
            }

            htmlBuilder.Append("</table></body></html>");

            return htmlBuilder.ToString();
        }
    }
}
