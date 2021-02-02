using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace SqlDataProviderSslHttp2
{
    public class RSql : Hub<ISqlMethods>
    {
        private static SqlConnection con;
        private static SqlCommand cmd;
        private static SqlDataReader dataReader;
        public async Task OpenConnection()
        {
            con = new SqlConnection(ISqlMethods.HackyConString);
            con.Open();
        }

        public async Task CreateCommand()
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "select top 1 ProductCategoryID FROM [SalesLT].[ProductCategory]";
        }

        public async Task ExecuteReader()
        {
            dataReader = cmd.ExecuteReader();
        }

        public async Task<byte[]> GetRows()
        {
            var rows = new List<object[]>();
            int fieldCount = dataReader.FieldCount;
            while (dataReader.Read())
            {
                object[] rowValues = new object[fieldCount];
                for (int i = 0; i < fieldCount; i++)
                {
                    rowValues[i] = dataReader.GetValue(i);
                }

                rows.Add(rowValues);
            }
            byte[] responseBytes = SerializeObjectToBinary(rows);
            return responseBytes;
        }


        public async Task CloseReader()
        {
            dataReader.Close();
        }

        public async Task CloseConnection()
        {
            con.Close();
        }

        private byte[] SerializeObjectToBinary(object value)
        {
            using (var stream = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(stream, value);
                return stream.ToArray();
            }
        }
      
    }
}
