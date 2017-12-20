using System.Collections.Generic;
using GoogleGeoApi.Models.DistanceMatrix;

namespace GoogleGeoApi
{
    public class DistanceMatrixClient<T>
    {
        private ApiClient _client = null;

        public DistanceMatrixClient(ApiClient client)
        {
            _client = client;
        }

        public T GetDistance(DistanceMatrixParameters parameters)
        {
            return _client.GetRequest<T>(parameters.GetParamsDictionary());
        }
    }
}
