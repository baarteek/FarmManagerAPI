using FarmManagerAPI.DTOs;
using FarmManagerAPI.Repositories.Interfaces;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.Extensions.Primitives;
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

            htmlBuilder.Append("<html><head>");
            htmlBuilder.Append("<title>Report</title>");
            htmlBuilder.Append("<style>");
            htmlBuilder.Append("table { width: 100%; border-collapse: collapse; } ");
            htmlBuilder.Append("th, td { border: 1px solid black; padding: 10px; text-align: center; vertical-align: middle; } ");
            htmlBuilder.Append("th { background-color: #f2f2f2; } ");
            htmlBuilder.Append("</style></head><body>");

            htmlBuilder.Append("<table>");
            htmlBuilder.Append("<tr> <th colspan=\"10\">WYKAZ DZIAŁAŃ AGROTECHNICZNYCH</th> </tr>");
            htmlBuilder.Append("<tr>");
            htmlBuilder.Append("<th>Oznaczenie działki (literowe)</th>");
            htmlBuilder.Append("<th>Numer działki ewidencyjnej</th>");
            htmlBuilder.Append("<th>Data wykonania czynności [dd/mm/rrrr]</th>");
            htmlBuilder.Append("<th>Powierzchnia działki/uprawy [ha,a]</th>");
            htmlBuilder.Append("<th>Rodzaj użytkowania (uprawa w plonie głównym/uprawa w poplonie)</th>");
            htmlBuilder.Append("<th>Rodzaj wykonywanej czynności*</th>");
            htmlBuilder.Append("<th>Nazwa środka ochrony roślin</th>");
            htmlBuilder.Append("<th>Zastosowana ilość środka ochrony roślin/nawozu</th>");
            htmlBuilder.Append("<th>Działanie/interwencja/praktyka Nummer pakietu lub wariantu**</th>");
            htmlBuilder.Append("<th>Uwagi/powierzchnia wykonywanej czynności***</th>");
            htmlBuilder.Append("</tr>");

            htmlBuilder.Append("<tr>");
            for (int i = 1; i <= 10; i++)
            {
                htmlBuilder.Append($"<th>{i}</th>");
            }
            htmlBuilder.Append("</tr>");

            foreach (var aa in agrtechnicalActivities)
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

            htmlBuilder.Append(AddFooterToReportHTML());
            htmlBuilder.Append("</table></body></html>");

            return htmlBuilder.ToString();
        }

        private string AddFooterToReportHTML()
        {
            var footer = new StringBuilder();

            footer.Append("<tr><td colspan=\"10\" style=\"padding: 10px; text-align: left;\">");
            footer.Append("* należy umieścić zapisy dotyczące: zabiegów agrotechnicznych, pielęgnacyjnych i zabiegów wykonanych środkami ochrony roślin, nawożenia i innych zabiegów wykonywanych na danej działce (rolnej)<br>");
            footer.Append("** wpisć działanie/ interwencję odpowiednią dla oznaczenia wpisanego w kolumnie \"Pakiety/warianty/ interwencje realizowane w gospodarstwie\" , przy czym dla Działania rolno-środowiskowo-klimatycznego PROW 2014-2020 wpisać PRSK1420, dla Rolnictwa ekologicznego wpisać RE1420, dla Płatnosci rolno-środowiskowo-klimatycznych WPR PS wpisać ZRSK2327, dla Rolnictwa ekologicznego WPR PS wpisać RE2327, praktyka Międzyplony ozime lub wsiewki środplonowe wpisac E_MPW, praktyka Opracowanie i przestrzeganie planu nawożenia: wariant podstawowy lub wariant z wapnowaniem wpisać E_OPN, Uproszczone systemy uprawy wpisać E_USU, Wymieszanie słomy z glebą  wpisać E_WSG, Biologiczna ochrona upraw wpisać E_BOU<br>");
            footer.Append("***należy wypełnić, gdy dana czynność lub zabieg nie są wykonywane na całej powierzchni działki (np. gdy koszeniu podlega 20% pow. działki), bądź w celu uszczegółowienia zapisów znajdujących się w innych kolumnach tego wiersza np. wskazanie sposobu realizacji integrowanej ochrony roślin (podanie co najmniej przyczyny wykonania zabiegu środkiem ochrony roślin) </td></tr>");

            footer.Append("<tr>");
            footer.Append("<td rowspan=\"3\" style=\"vertical-align: top; padding: 15px; background-color: lightgray;\">Pole wypełniane podczas kontroli na miejscu</td>");
            footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\">Data kontroli</td>");
            footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\">Nazwisko i imię inspektora</td>");
            footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\">Podpis inspektora terenowego</td>");
            footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\">Nazwisko i imię osoby obecnej przy kontroli</td>");
            footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\">Podpis osoby obecnej przy kontroli</td>");
            footer.Append("</tr>");

            for (int i = 0; i < 2; i++)
            {
                footer.Append("<tr>");
                footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\"></td>");
                footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\"></td>");
                footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\"></td>");
                footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\"></td>");
                footer.Append("<td colspan=\"2\" style=\"height: 40px; background-color: lightgray;\"></td>");
                footer.Append("</tr>");
            }

            return footer.ToString();
        }
    }
}
