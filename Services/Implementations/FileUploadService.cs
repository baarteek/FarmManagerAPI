using System.Xml.Linq;
using FarmManagerAPI.Services.Interfaces;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;
using Newtonsoft.Json;

namespace FarmManagerAPI.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly ICropRepository _cropRepository;

        private const string GmlNamespace = "http://www.opengis.net/gml/3.2";

        public FileUploadService(
            IFarmRepository farmRepository, 
            IFieldRepository fieldRepository, 
            ICropRepository cropRepository)
        {
            _farmRepository = farmRepository;
            _fieldRepository = fieldRepository;
            _cropRepository = cropRepository;
        }

        public async Task ReadFileContent(IFormFile file, Guid farmId)
        {
            ValidateFile(file);

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                var gmlDoc = XDocument.Load(stream);

                var crops = gmlDoc.Descendants().Where(x => x.Name.LocalName == "uprawa");

                foreach (var crop in crops)
                {
                    await ProcessCrop(crop, farmId);
                }
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file provided or the file is empty.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".gml")
            {
                throw new ArgumentException("Invalid file extension. Only .gml files are allowed.");
            }
        }

        private async Task ProcessCrop(XElement crop, Guid farmId)
        {
            var coordinates = GetCoordinates(crop);
            if (coordinates == null)
            {
                throw new InvalidOperationException("No coordinates found for a crop.");
            }

            if (!double.TryParse(crop.Elements().FirstOrDefault(e => e.Name.LocalName == "powierzchnia")?.Value, out double area))
            {
                throw new InvalidOperationException("Invalid area value. Skipping this crop.");
            }

            var plantType = crop.Elements().FirstOrDefault(e => e.Name.LocalName == "roslina_uprawna")?.Value ?? "Unknown Plant";
            var cropIdentifier = crop.Elements().FirstOrDefault(e => e.Name.LocalName == "oznaczenie_uprawy")?.Value ?? "Unknown Identifier";

            var fieldId = await _fieldRepository.GetFieldIdByCoordinates(JsonConvert.SerializeObject(coordinates));

            if (fieldId != Guid.Empty)
            {
                await HandleExistingField(fieldId, cropIdentifier, plantType);
            }
            else
            {
                await CreateNewFieldAndCrop(farmId, coordinates, area, cropIdentifier, plantType);
            }
        }

        private List<List<List<double>>> GetCoordinates(XElement crop)
        {
            var geometry = crop.Descendants().FirstOrDefault(x => x.Name.LocalName == "Polygon");
            var posList = geometry?.Descendants().FirstOrDefault(x => x.Name.LocalName == "posList")?.Value;
            return !string.IsNullOrEmpty(posList) ? ParseCoordinatesToGeoJson(posList) : null;
        }

        private async Task HandleExistingField(Guid fieldId, string cropIdentifier, string plantType)
        {
            var existingCropId = await _cropRepository.GetCropIdByIdentifierAndFieldId(cropIdentifier, fieldId);
            if (existingCropId != Guid.Empty)
            {
                var existingCrop = await _cropRepository.GetById(existingCropId);
                if (existingCrop != null)
                {
                    existingCrop.Name = plantType;
                    existingCrop.CropIdentifier = cropIdentifier;
                    await _cropRepository.Update(existingCrop);
                }
            }
            else
            {
                var newCrop = new Crop
                {
                    Field = await _fieldRepository.GetById(fieldId),
                    CropIdentifier = cropIdentifier,
                    Name = plantType
                };
                await _cropRepository.Add(newCrop);
            }
        }

        private async Task CreateNewFieldAndCrop(Guid farmId, List<List<List<double>>> coordinates, double area, string cropIdentifier, string plantType)
        {
            var newField = new Field
            {
                Id = Guid.NewGuid(),
                Name = "Field",
                Farm = await _farmRepository.GetById(farmId),
                Coordinates = JsonConvert.SerializeObject(coordinates),
                Area = area,
                SoilType = SoilType.NotSelected
            };

            await _fieldRepository.Add(newField);

            var newCrop = new Crop
            {
                Id = Guid.NewGuid(),
                Field = newField,
                CropIdentifier = cropIdentifier,
                Name = plantType,
                Type = CropType.NotSelected,
                IsActive = true
            };

            await _cropRepository.Add(newCrop);
        }

        private List<List<List<double>>> ParseCoordinatesToGeoJson(string posList)
        {
            var coordinates = posList
                .Split(' ')
                .Select((val, index) => new { val, index })
                .GroupBy(x => x.index / 2)
                .Select(g => new List<double> { double.Parse(g.ElementAt(0).val), double.Parse(g.ElementAt(1).val) })
                .ToList();

            return new List<List<List<double>>> { new List<List<double>>(coordinates) };
        }
    }
}
