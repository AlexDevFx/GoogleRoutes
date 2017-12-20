using Newtonsoft.Json;

namespace GoogleGeoApi.Models.Geocoding
{
    public class GeocodingResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("results")]
        public GeocodingResult[] Results;
    }

    public class GeocodingResult
    {
        [JsonProperty("address_components")]
        public AddressComponent[] AddressComponents { get; set; }

        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public class AddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("types")]
        public string[] Types { get; set; }
    }

    public class Geometry
    {
        [JsonProperty("location")]
        public GeoCoordinates Location { get; set; }

        [JsonProperty("location_type")]
        public string LocationType { get; set; }

        [JsonProperty("viewport")]
        public GeoViewport Viewport { get; set; }
    }

    public class GeoViewport
    {
        [JsonProperty("northeast")]
        public GeoCoordinates Northeast { get; set; }

        [JsonProperty("southwest")]
        public GeoCoordinates Southwest { get; set; }
    }
}
