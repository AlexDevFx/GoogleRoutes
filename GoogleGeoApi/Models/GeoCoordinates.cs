using Newtonsoft.Json;
using System.Globalization;

namespace GoogleGeoApi.Models.Geocoding
{
    public class GeoCoordinates
    {
        [JsonProperty("lat")]
        public double Latitude { get; }

        [JsonProperty("lng")]
        public double Longitude { get; }

        public GeoCoordinates(double latitude, double longitude)
        {
            Longitude = longitude;
            Latitude = latitude;
        }

        public override string ToString()
        {
            return string.Format(new CultureInfo("en-US"), "{0:f8},{1:f8}", Latitude, Longitude);
        }
    }
}
