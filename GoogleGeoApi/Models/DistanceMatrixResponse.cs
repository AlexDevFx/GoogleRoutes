using Newtonsoft.Json;

namespace GoogleGeoApi.Models.DistanceMatrix
{
    public class DistanceMatrixResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("destination_addresses")]
        public string[] DestinationAddresses { get; set; }

        [JsonProperty("origin_addresses")]
        public string[] OriginAddresses { get; set; }

        [JsonProperty("rows")]
        public Row[] Rows { get; set; }
    }

    public class Row
    {
        [JsonProperty("elements")]
        public Element[] Elements { get; set; }
    }

    public class Element
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("distance")]
        public ElementItem Distance { get; set; }

        [JsonProperty("Duration")]
        public ElementItem Duration { get; set; }
    }

    public class ElementItem
    {
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
