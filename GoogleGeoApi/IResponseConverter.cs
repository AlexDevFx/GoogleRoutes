
namespace GoogleGeoApi
{
    public interface IResponseConverter
    {
        T Convert<T>(string input);
    }
}
