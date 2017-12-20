using System;
using GoogleGeoApi;
using Microsoft.Extensions.Configuration;
using PennedObjects.RateLimiting;
using GoogleGeoApi.Models.DistanceMatrix;
using GoogleGeoApi.Models.Geocoding;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

namespace GoogleRoutes
{
    public class DistanceCalculator
    {
        private GeocodingClient<GeocodingResponse> geocodingClient;
        private DistanceMatrixClient<DistanceMatrixResponse> distanceMatrixClient;
        private string _configFileName;
        private string _outputFileName;

        public DistanceCalculator(string configFileName, string outputFileName)
        {
            _configFileName = configFileName;
            _outputFileName = outputFileName;
        }

        public void DoWork()
        {
            Initialize();
            Console.WriteLine("Please, enter coordinates in form <latitude>, <longitude>. For ex.:37.4224764, -122.0842499.");
            Console.WriteLine("Enter <Q> when you done input.");

            Console.WriteLine("Origin coordinates:");
            string[] origins = ReadConsoleStrings();
            Console.WriteLine("Destination coordinates:");
            string[] destinations = ReadConsoleStrings();

            DistanceMatrixParameters distanceParameters = new DistanceMatrixParameters
            {
                Origins = new string[] { string.Join('|', origins) },
                Destinactions = new string[] { string.Join('|', destinations) },
                Units = "metric",
                Mode = "driving",
                Language = "en-US"
            };

            DistanceMatrixResponse distanceResponse = distanceMatrixClient.GetDistance(distanceParameters);

            OutputDistanceResponse(distanceResponse);
            WriteToFile(_outputFileName, distanceResponse);

            Console.WriteLine("Please, press any key to exit");
            Console.ReadKey();
        }

        private void Initialize()
        {
            var configBuilder = new ConfigurationBuilder().AddJsonFile(_configFileName);
            var config = configBuilder.Build();

            geocodingClient = new GeocodingClient<GeocodingResponse>(new ApiClient(new ApiContext
            {
                OutputFormat = config["GoogleApi:OutputFormat"],
                ApiKey = config["GoogleApi:GeocodingApiKey"],
                HostName = config["GoogleApi:HostName"],
                Path = config["GoogleApi:Path"],
                Protocol = config["GoogleApi:Protocol"],
                ApiMethod = config["GoogleApi:GeocodingMethod"]
            }, new RateGate(1, TimeSpan.FromSeconds(5)), new JsonResponseConverter()));

            distanceMatrixClient = new DistanceMatrixClient<DistanceMatrixResponse>(new ApiClient(new ApiContext
            {
                OutputFormat = config["GoogleApi:OutputFormat"],
                ApiKey = config["GoogleApi:DistanceMatixApiKey"],
                HostName = config["GoogleApi:HostName"],
                Path = config["GoogleApi:Path"],
                Protocol = config["GoogleApi:Protocol"],
                ApiMethod = config["GoogleApi:DistanceMatrixMethod"]
            }, new RateGate(1, TimeSpan.FromSeconds(5)), new JsonResponseConverter()));
        }

        private string[] ReadConsoleStrings()
        {
            List<string> strings = new List<string>();
            string input = "";

            while ((input = Console.ReadLine()).ToUpper().IndexOf("Q") < 0)
            {
                strings.Add(input);
            }

            return strings.ToArray();
        }

        private GeocodingResponse FindAddress(GeoCoordinates location)
        {
            return geocodingClient.ReverseGeocoding(location);
        }

        private bool IsFoundAddress(GeoCoordinates location)
        {
            GeocodingResponse response = FindAddress(location);
            bool is_found = IsStatusOk(response.Status);

            if (!is_found)
                Console.WriteLine($"Address of location: {location.ToString()} is not found: {response.Status}");

            return is_found;
        }

        private GeoCoordinates ParseToCoordinates(string input)
        {
            double latitude = 0.0, longitude = 0.0;

            string[] strCoordinates = input.Split(',');

            if (strCoordinates.Length > 1)
            {
                try
                {
                    latitude = double.Parse(strCoordinates[0].Trim());
                    longitude = double.Parse(strCoordinates[1].Trim());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Parsing coodinates error: {ex.Message}");
                }
            }

            return new GeoCoordinates(latitude, longitude);
        }

        private void OutputDistanceResponse(DistanceMatrixResponse response)
        {
            if (IsStatusOk(response.Status))
            {
                int i = 0;
                Console.WriteLine($"{string.Join('/', response.OriginAddresses)}-->{string.Join('/', response.DestinationAddresses)}");
                foreach (var row in response.Rows)
                {
                    foreach (var element in row.Elements)
                    {
                        if (IsStatusOk(element.Status))
                        {
                            Console.WriteLine($"#{++i}=>Distance: {element.Distance.Text}, Duration: {element.Duration.Text}");
                        }
                        else
                        {
                            Console.WriteLine($"Result: {element.Status}");
                            return;
                        }
                    }
                }
            }
            else
                Console.WriteLine($"Response error: {response.Status}");
        }

        private void WriteToFile(string fileName, object response)
        {
            JsonSerializer serializer = new JsonSerializer();

            using (StreamWriter sw = new StreamWriter(fileName))
            using (JsonWriter writer = new JsonTextWriter(sw))
                serializer.Serialize(writer, response);
        }

        private bool IsStatusOk(string status)
        {
            return status == "OK";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            DistanceCalculator distanceCalculator = new DistanceCalculator("app.json", "distance.json");
            distanceCalculator.DoWork();
        }
    }
}
