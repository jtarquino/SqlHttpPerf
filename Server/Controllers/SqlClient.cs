using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SqlDataProviderSslHttp2.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SqlClientController : ControllerBase
    {

        private static SqlConnection con;
        private static SqlCommand cmd;
        private static SqlDataReader dataReader;

        [HttpGet]
        public string Validate()
        {
            return "helloWorld";
        }


        [HttpGet]
        [Route("OpenConnection")]
        public async Task<HttpResponseMessage> OpenConnection()
        {
            con = new SqlConnection(ISqlMethods.HackyConString);
            con.Open();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [HttpGet]
        [Route("CreateCommand")]
        public async Task<HttpResponseMessage> CreateCommand()
        {
            cmd = con.CreateCommand();
            cmd.CommandText = "select top 1 ProductCategoryID FROM [SalesLT].[ProductCategory]";
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("ExecuteReader")]
        [HttpGet]
        public async Task<HttpResponseMessage> ExecuteReader()
        {
            dataReader = cmd.ExecuteReader();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("GetRows")]
        [HttpGet]
        public async Task<HttpResponseMessage> GetRows()
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
            var response = CreateByteArrayResponse(responseBytes);

            return await Task.FromResult(response).ConfigureAwait(false);
        }

        [Route("CloseReader")]
        [HttpGet]
        public async Task<HttpResponseMessage> CloseReader()
        {
            dataReader.Close();
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("CloseConnection")]
        [HttpGet]
        public async Task<HttpResponseMessage> CloseConnection()
        {
            con.Close();
            return new HttpResponseMessage(HttpStatusCode.OK);
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
        private HttpResponseMessage CreateByteArrayResponse(byte[] bytes)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            if (bytes != null)
            {
                response.Content = new ByteArrayContent(bytes);
            }

            return response;
        }
    }
}
