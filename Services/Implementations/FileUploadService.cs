using System.Text;
using System.Xml.Linq;
using FarmManagerAPI.Repositories.Implementations;
using FarmManagerAPI.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using FarmManagerAPI.Models;
using FarmManagerAPI.Models.Enums;
using FarmManagerAPI.Repositories.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class FileUploadService : IFileUploadService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly ICropRepository _cropRepository;

        public FileUploadService(IFarmRepository farmRepository, IFieldRepository fieldRepository, ICropRepository cropRepository)
        {
            _farmRepository = farmRepository;
            _fieldRepository = fieldRepository;
            _cropRepository = cropRepository;
        }

        public async Task<string> ReadFileContentAsync(IFormFile file, Guid farmId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("No file provided or the file is empty.");

            if (Path.GetExtension(file.FileName).ToLower() != ".gml")
                throw new ArgumentException("Only .gml files are allowed.");

            using (var stream = new MemoryStream())
            {
                try
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;
                
                    XDocument gmlDoc = XDocument.Load(stream);
                    var gmlNs = "http://www.opengis.net/gml/3.2";
                    var uprawy = gmlDoc.Descendants().Where(x => x.Name.LocalName == "uprawa");

                    foreach (var uprawa in uprawy)
                    {
                        try
                        {
                            var geometry = uprawa.Descendants().FirstOrDefault(x => x.Name.LocalName == "Polygon");
                            var coordinates = geometry?.Descendants().FirstOrDefault(x => x.Name.LocalName == "posList")?.Value;
                            if (coordinates == null)
                            {
                                Console.WriteLine("No coordinates found for a crop.");
                                continue;
                            }

                            var areaElement = uprawa.Elements().FirstOrDefault(e => e.Name.LocalName == "powierzchnia")?.Value;
                            if (!double.TryParse(areaElement, out double area))
                            {
                                Console.WriteLine("Invalid area value. Skipping this crop.");
                                continue;
                            }

                            var plantType = uprawa.Elements().FirstOrDefault(e => e.Name.LocalName == "roslina_uprawna")?.Value ?? "Unknown Plant";
                            var cropIdentifier = uprawa.Elements().FirstOrDefault(e => e.Name.LocalName == "oznaczenie_uprawy")?.Value ?? "Unknown Identifier";
                    
                            var fieldId = await _fieldRepository.GetFieldIdByCoordinates(coordinates);
                            if (fieldId != Guid.Empty)
                            {
                                var existingCropId = await _cropRepository.GetCropIdByIdentifierAndFieldId(cropIdentifier, fieldId);
                                if (existingCropId == Guid.Empty)
                                {
                                    var existingCrop = await _cropRepository.GetById(existingCropId);
                                    Console.WriteLine("CropID: "  + existingCropId);
                                    if (existingCrop != null)
                                    {
                                        existingCrop.Name = plantType;
                                        existingCrop.CropIdentifier = cropIdentifier;
                                        await _cropRepository.Update(existingCrop);
                                    }
                                    else
                                    {
                                        Console.WriteLine("Crop not found, even though ID was returned. Skipping update.");
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
                            else
                            {
                                var newField = new Field
                                {
                                    Id = Guid.NewGuid(),
                                    Name = "Field",
                                    Farm = await _farmRepository.GetById(farmId),
                                    Coordinates = coordinates,
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
                                    IsActive = true,
                                };
                                await _cropRepository.Add(newCrop);
                            }
                        }
                        catch (Exception innerEx)
                        {
                            Console.WriteLine($"Error processing crop: {innerEx.Message}");
                        }
                    }

                    return "Import and processing completed successfully.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error reading file content: {ex.Message}");
                    return $"Error reading file content: {ex.Message}";
                }
            }
        }
    }
}
