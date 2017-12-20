using Newtonsoft.Json;

namespace GoogleGeoApi
{
    public class JsonResponseConverter : IResponseConverter
    {
        public T Convert<T>(string input)
        {
            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
