using FarmManagerAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FarmManagerAPI.Repositories.Interfaces
{
    public interface ISoilMeasurementRepository : IGenericRepository<SoilMeasurement>
    {
        Task<IEnumerable<SoilMeasurement>> GetSoilMeasurementsByFieldId(Guid fieldId);
    }
}
