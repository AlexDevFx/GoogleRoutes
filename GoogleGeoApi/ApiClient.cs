using PennedObjects.RateLimiting;
using System.Collections.Generic;

namespace GoogleGeoApi
{
    public class ApiClient
    {
        private ApiRequest _apiRequest = new ApiRequest();
        private IResponseConverter _converter;

        public ApiClient(ApiContext context, RateGate rateGate, IResponseConverter converter)
        {
            _apiRequest = new ApiRequest(context, rateGate);
            _converter = converter;
        }

        public T GetRequest<T>(Dictionary<string, string> parameters)
        {
            foreach (var p in parameters)
            {
                _apiRequest.AddParameter(p.Key, p.Value);
            }
            string responseStr = _apiRequest.Get();

            return _converter.Convert<T>(responseStr);
        }
    }
}
