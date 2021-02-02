using System.Net.Http;
using System.Threading.Tasks;


namespace SqlDataProviderSslHttp2
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
}