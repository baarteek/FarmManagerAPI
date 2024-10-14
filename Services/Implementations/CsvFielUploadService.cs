using CsvHelper;
using System.Globalization;
using Microsoft.AspNetCore.Http;
using System.IO;
using FarmManagerAPI.Services.Interfaces;

namespace FarmManagerAPI.Services.Implementations
{
    public class CsvFileUploadService : ICsvFileUploadService
    {
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
                        var oznaczenieUprawy = csv.GetField("Oznaczenie Uprawy / działki rolnej");
                        var nrDzialkiEwidencyjnej = csv.GetField("Nr działki ewidencyjnej");
                        var powierzchnia = csv.GetField("Powierzchnia uprawy w granicach działki ewidencyjnej - ha");

                        Console.WriteLine($"Oznaczenie Uprawy / działki rolnej: {oznaczenieUprawy}");
                        Console.WriteLine($"Nr działki ewidencyjnej: {nrDzialkiEwidencyjnej}");
                        Console.WriteLine($"Powierzchnia uprawy w granicach działki ewidencyjnej (ha): {powierzchnia}");
                        Console.WriteLine("--------------------------------------------------");
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
    }
}
