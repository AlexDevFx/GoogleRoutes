using System.Collections.Generic;
using GoogleGeoApi.Models.Geocoding;

namespace GoogleGeoApi
{
    public class GeocodingClient<T>
    {
        private ApiClient _client = null;

        public GeocodingClient(ApiClient client)
        {
            _client = client;
        }

        public T Geocoding(string address)
        {
            return _client.GetRequest<T>(new Dictionary<string, string>() { {"address", address} });
        }

        public T ReverseGeocoding(GeoCoordinates location)
        {
            return _client.GetRequest<T>(new Dictionary<string, string>() { {"latlng", location.ToString()} });
        }

        public T ReverseGeocoding(string placeId)
        {
            return _client.GetRequest<T>(new Dictionary<string, string>() { {"place_id", placeId} });
        }
    }
}
