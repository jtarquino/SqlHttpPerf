using System.Net.Http;
using System.Threading.Tasks;


namespace HttpHandler
{
    public interface ISqlMethods
    {
        Task CloseConnection();
        Task CloseReader();
        Task CreateCommand();
        Task ExecuteReader();
        Task<byte[]> GetRows();
        Task OpenConnection();
    }

    public class ConString
    {
        public const string HackyConString = "YourOwnConnectionString";
    }
}