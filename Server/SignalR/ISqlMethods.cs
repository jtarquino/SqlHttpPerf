using System.Net.Http;
using System.Threading.Tasks;


namespace SqlDataProviderSslHttp2
{
    public interface ISqlMethods
    {
        Task OpenConnection();
        Task CreateCommand();
        Task ExecuteReader();
        Task CloseReader();
        Task<byte[]> GetRows();
        Task CloseConnection();


        public const string HackyConString = "YourConnectionString";
    }
}