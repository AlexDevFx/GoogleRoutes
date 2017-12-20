using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleGeoApi.Models.DistanceMatrix
{
    public class DistanceMatrixParameters
    {
        public string[] Origins { get; set; }
        public string[] Destinactions { get; set; }
        public string Units { get; set; }
        public string Mode { get; set; }
        public string Language { get; set; }

        public Dictionary<string, string> GetParamsDictionary()
        {
            return new Dictionary<string, string>()
            {
                { "origins", string.Join('|', Origins) },
                { "destinations", string.Join('|', Destinactions) },
                { "units", Units },
                { "mode", Mode },
                { "language", Language }
            };
        }
    }
}
