using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.IO;
using FarmManagerAPI.Services.Interfaces;
using FarmManagerAPI.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using FarmManagerAPI.DTOs;
using FarmManagerAPI.Models;

namespace FarmManagerAPI.Services.Implementations
{
    public class CsvFileUploadService : ICsvFileUploadService
    {
        private readonly IFarmRepository _farmRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly ICropRepository _cropRepository;
        private readonly IReferenceParcelRepository _referenceParcelRepository;

        public CsvFileUploadService(IFarmRepository farmRepository, IFieldRepository fieldRepository, ICropRepository cropRepository, IReferenceParcelRepository referenceParcelRepository)
        {
            _farmRepository = farmRepository;
            _fieldRepository = fieldRepository;
            _cropRepository = cropRepository;
            _referenceParcelRepository = referenceParcelRepository;
        }

        public async Task ReadFileContent(IFormFile file, Guid farmId)
        {
            ValidateFile(file);

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; 

                using (var reader = new StreamReader(stream))
                using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    if (await csv.ReadAsync())
                    {
                        csv.ReadHeader();
                    }

                    while (await csv.ReadAsync())
                    {
                        var cropIdentifier = csv.GetField("Oznaczenie Uprawy / działki rolnej");
                        var parcelNumber = csv.GetField("Nr działki ewidencyjnej");
                        var area = csv.GetField("Powierzchnia uprawy w granicach działki ewidencyjnej - ha");

                       await AddReferenceParcelToField(cropIdentifier, parcelNumber, Double.Parse(area));
                    }
                }
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentNullException("No file provided or the file is empty.");
            }

            if (Path.GetExtension(file.FileName).ToLower() != ".csv")
            {
                throw new ArgumentException("Invalid file extension. Only .csv files are allowed.");
            }
        }

        private async Task AddReferenceParcelToField(string cropIdentifier, string parcelNumber, double area)
        {
            if(cropIdentifier.IsNullOrEmpty() || parcelNumber.IsNullOrEmpty())
            {
                return;
            }

            var fieldId = await _cropRepository.GetFieldIdByCropIdentifier(cropIdentifier);
            if(fieldId.Equals(Guid.Empty))
            {
                return; 
            }

            var referenceParcel = await _referenceParcelRepository.GetReferenceParcelByNumberAndFieldId(parcelNumber, fieldId);
            if(referenceParcel.Equals(Guid.Empty))
            {
                var rp = new ReferenceParcel
                {
                    Id = Guid.NewGuid(),
                    Field = await _fieldRepository.GetById(fieldId),
                    ParcelNumber = parcelNumber,
                    Area = area
                };
                await _referenceParcelRepository.Add(rp);
                Console.WriteLine("Dodano numer dzialki");
            }
        }
    }
}
