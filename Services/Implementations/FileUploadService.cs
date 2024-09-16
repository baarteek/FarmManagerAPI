using System.Text;
using System.Xml.Linq;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FarmManagerAPI.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        public async Task<string> ReadFileContentAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided or the file is empty.");

            if (Path.GetExtension(file.FileName).ToLower() != ".gml")
                throw new ArgumentException("Only .gml files are allowed.");

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                
                XDocument gmlDoc = XDocument.Load(stream);
                
                StringBuilder sb = new StringBuilder();
                var gmlNs = "http://www.opengis.net/gml/3.2";

                var crops = gmlDoc.Descendants().Where(x => x.Name.LocalName == "uprawa");

                foreach (var crop in crops)
                {
                    var geometry = crop.Descendants().Where(x => x.Name.LocalName == "Polygon").FirstOrDefault();
                    if (geometry != null)
                    {
                        var posList = geometry.Descendants().Where(x => x.Name.LocalName == "posList").FirstOrDefault()?.Value;
                        var srsName = geometry.Attribute("srsName")?.Value;
                        sb.AppendLine("Geometria:");
                        sb.AppendLine($" - SRS: {srsName}");
                        sb.AppendLine($" - Współrzędne: {posList}");
                    }
                    
                    var area = crop.Elements().FirstOrDefault(e => e.Name.LocalName == "powierzchnia")?.Value;
                    sb.AppendLine($"Powierzchnia: {area}");
                    
                    var cropDesignation = crop.Elements().FirstOrDefault(e => e.Name.LocalName == "oznaczenie_uprawy")?.Value;
                    sb.AppendLine($"Oznaczenie Uprawy: {cropDesignation}");
                    
                    var plant = crop.Elements().FirstOrDefault(e => e.Name.LocalName == "roslina_uprawna")?.Value;
                    sb.AppendLine($"Roślina Uprawna: {plant}");

                    sb.AppendLine(new string('\n', 40));
                }

                return sb.ToString();
            }
        }
    }
}
