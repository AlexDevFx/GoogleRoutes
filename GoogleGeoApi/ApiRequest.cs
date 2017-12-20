using System;
using System.Net;
using System.IO;
using PennedObjects.RateLimiting;
using System.Collections.Specialized;
using System.Web;

namespace GoogleGeoApi
{
    public class ApiContext
    {
        public string ApiKey { get; set; }
        public string HostName { get; set; }
        public string Path { get; set; }
        public string Protocol { get; set; }
        public string OutputFormat { get; set; }
        public string ApiMethod { get; set; }
    }

    public class ApiRequest : IDisposable
    {
        private ApiContext _apiContext;
        private RateGate   _rateGate = new RateGate(1, TimeSpan.FromSeconds(5));
        private NameValueCollection _requestParameters = HttpUtility.ParseQueryString(string.Empty);

        public ApiRequest()
        {
        }

        public ApiRequest(ApiContext context, RateGate rateGate )
        {
            _apiContext = context;
            _rateGate = rateGate;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_rateGate != null)
                    _rateGate.Dispose();
            }

            _rateGate = null;
        }

        ~ApiRequest()
        {
            Dispose(false);
        }

        public void AddParameter(string name, string value)
        {
            _requestParameters[name] = value;
        }

        private string SendRequest(HttpWebRequest webRequest)
        {
            try
            {
                _rateGate.WaitToProceed();

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (Stream str = webResponse.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (WebException wex)
            {
                using (HttpWebResponse response = (HttpWebResponse)wex.Response)
                {
                    using (Stream str = response.GetResponseStream())
                    {
                        using (StreamReader sr = new StreamReader(str))
                        {
                            if (response.StatusCode != HttpStatusCode.InternalServerError)
                            {
                                throw new WebException(String.Format("Web exception: {0}", wex.Message));
                            }
                            return sr.ReadToEnd();
                        }
                    }
                }

            }
        }

        public string Get()
        {
            return SendRequest(CreateHttpRequest("GET"));
        }

        public HttpWebRequest CreateHttpRequest(string httpMethod)
        {
            string requestStr = BuildRequestString();
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(requestStr);
            webRequest.Method = httpMethod;
            _requestParameters.Clear();

            return webRequest;
        }

        private string BuildRequestString()
        {
            UriBuilder uriBuilder = new UriBuilder(_apiContext.Protocol, _apiContext.HostName );
            uriBuilder.Path = String.Format("{0}/{1}/{2}", _apiContext.Path, _apiContext.ApiMethod, _apiContext.OutputFormat);
            
            _requestParameters["key"] = _apiContext.ApiKey;
            uriBuilder.Query = _requestParameters.ToString();
            
            return uriBuilder.Uri.ToString();
        }
    }
}
