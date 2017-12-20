using GoogleGeoApi.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using PennedObjects.RateLimiting;
using GoogleGeoApi.Models.DistanceMatrix;
using GoogleGeoApi.Models.Geocoding;

namespace GoogleGeoApi.Test
{
    [TestClass]
    public class GeocodingTest
    {
        private GeocodingClient<GeocodingResponse> _geocodingClient = new GeocodingClient<GeocodingResponse>(new ApiClient(new ApiContext
        {
            OutputFormat = "json",
            ApiKey = "AIzaSyCkHeVqS4I0mij4HY2sggQ75ssUXbwGXNg",
            HostName = "maps.googleapis.com",
            Path = "maps/api",
            Protocol = "https",
            ApiMethod = "geocode"
        }, new RateGate(1, TimeSpan.FromSeconds(5)), new JsonResponseConverter() ));

        private readonly string _addressStr = "1600 Amphitheatre Pkwy, Mountain View, CA";
        private readonly GeoCoordinates _locationCoordinates = new GeoCoordinates(37.4224764, -122.0842499);
        private readonly string _placeId = "ChIJ2eUgeAK6j4ARbn5u_wAGqWA";

        [TestMethod]
        public void Geocoding_GoogleBuilidng_statusOk()
        {
            GeocodingResponse response = _geocodingClient.Geocoding(_addressStr);
           
            Assert.AreEqual(IsStatusOk(response.Status), true);
        }

        [TestMethod]
        public void Geocoding_GoogleBuilidng_IsEqualCoordinates()
        {
            GeocodingResponse response = _geocodingClient.Geocoding(_addressStr);

            Assert.AreEqual(IsStatusOk(response.Status), true);
            Assert.AreEqual(IsCoordinatesFound(response.Results), true);
        }

        [TestMethod]
        public void ReverseGeocoding_Coordinates_IsGoogleBuilding()
        {
            GeocodingResponse response = _geocodingClient.ReverseGeocoding(_locationCoordinates);

            Assert.AreEqual(IsStatusOk(response.Status), true);
            Assert.AreEqual(IsAddressFound(response.Results), true);
        }

        [TestMethod]
        public void ReverseGeocoding_PlaceId_IsGoogleBuilding()
        {
            GeocodingResponse response = _geocodingClient.ReverseGeocoding(_placeId);

            Assert.AreEqual(IsStatusOk(response.Status), true);
            Assert.AreEqual(IsAddressFound(response.Results), true);
        }


        private bool IsStatusOk(string status)
        {
            return status.ToUpper() == "OK";
        }

        private bool IsAddressFound(GeocodingResult[] results)
        {
            var address = from r in results where r.FormattedAddress.IndexOf(_addressStr) > -1 select r;

            return address != null;
        }

        private bool IsCoordinatesFound(GeocodingResult[] results)
        {
            var coord = from r in results
                        where r.Geometry.Location.ToString().IndexOf(_locationCoordinates.Latitude.ToString()) > -1
                              && r.Geometry.Location.ToString().IndexOf(_locationCoordinates.Longitude.ToString()) > -1
                        select r;

            return coord != null;
        }
    }

    [TestClass]
    public class DistanceMatrixTest
    {

        private DistanceMatrixClient<DistanceMatrixResponse> _distanceClient = new DistanceMatrixClient<DistanceMatrixResponse>(new ApiClient(new ApiContext
        {
            OutputFormat = "json",
            ApiKey = "AIzaSyAbpuk2_LpQvM0SMzSTScd18DnSo2nmF0w",
            HostName = "maps.googleapis.com",
            Path = "maps/api",
            Protocol = "https",
            ApiMethod = "distancematrix"
        }, new RateGate(1, TimeSpan.FromSeconds(5)), new JsonResponseConverter()));

        private string[] _originAdresses = new string[] { "Vancouver, BC, Canada", "Seattle" };
        private string[] _destinationAddresses = new string[] {"San Francisco, CA, United States", "Victoria, BC, Canada"};
        private string _mode = "driving";
        private string _units = "metric";
        private string _language = "en-US";

        [TestMethod]
        public void Geocoding_GetDistance_statusOk()
        {
            DistanceMatrixParameters distanceParameters = new DistanceMatrixParameters
            {
                Origins = _originAdresses,
                Destinactions = _destinationAddresses,
                Units = _units,
                Mode = _mode,
                Language = _language
            };
            DistanceMatrixResponse response = _distanceClient.GetDistance(distanceParameters);
            Assert.AreEqual(IsStatusOk(response.Status), true);
        }

        private bool IsStatusOk(string status)
        {
            return status.ToUpper() == "OK";
        }
    }
}
