using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

public static class CoordinateConverter
{
    public static List<List<List<double>>> ConvertToWGS84(string posList)
    {
        var epsg2180CS = ProjectedCoordinateSystem.WGS84_UTM(33, true); // EPSG:2180
        var wgs84CS = GeographicCoordinateSystem.WGS84;

        var coordTransformFactory = new CoordinateTransformationFactory();
        var transform = coordTransformFactory.CreateFromCoordinateSystems(epsg2180CS, wgs84CS);
        
        var coordsArray = posList.Split(' ')
            .Select(double.Parse)
            .ToArray();

        var coordinates = new List<List<double>>();

        for (int i = 0; i < coordsArray.Length; i += 2)
        {
            var x = coordsArray[i];
            var y = coordsArray[i + 1];
            
            double[] geoCoords = transform.MathTransform.Transform(new double[] { x, y });
            coordinates.Add(new List<double> { geoCoords[1], geoCoords[0] });
        }

        return new List<List<List<double>>> { coordinates };
    }
}