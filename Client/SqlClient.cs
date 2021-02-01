using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;

namespace HttpHandler
{
    public class SqlClient
    {
        HttpClient client;
        private const string openConnection = "https://localhost:5001/SqlClient/OpenConnection";
        private const string createCommand = "https://localhost:5001/SqlClient/CreateCommand";
        private const string executeReader = "https://localhost:5001/SqlClient/ExecuteReader";
        private const string getRows = "https://localhost:5001/SqlClient/GetRows";
        private const string closeReader = "https://localhost:5001/SqlClient/CloseReader";
        private const string closeConnection = "https://localhost:5001/SqlClient/CloseConnection";
        public const string HackyConString = "replaceForConnectionstring";
        public void ReadData()
        {
            var sw = new Stopwatch();

            client = new HttpClient();
            sw.Reset();
            sw.Start();
            ReadDataFromHttp();
            Console.WriteLine($"Http1: {sw.ElapsedMilliseconds} milliseconds");

#if NETFRAMEWORK
            client = new HttpClient(new ForceHttp2Handler(new WinHttpHandler()));
#else
            client = new HttpClient()
            {
                DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
                DefaultRequestVersion = System.Net.HttpVersion.Version20
            };
#endif
            sw.Reset();
            sw.Start();
            ReadDataFromHttp();
            Console.WriteLine($"Http2: {sw.ElapsedMilliseconds} milliseconds");

            sw.Reset();
            sw.Start();
            ReadeDataFromSql();
            Console.WriteLine($"Sql Native: {sw.ElapsedMilliseconds} milliseconds");
        }

        private void ReadDataFromHttp()
        {
            for (int i = 0; i < 100; i++)
            {
                Execute(openConnection);
                Execute(createCommand);
                Execute(executeReader);
                Execute(getRows);
                Execute(closeReader);
                Execute(closeConnection);
                if (i % 10 == 0) { Console.WriteLine(i); }
            }
        }

        private void ReadeDataFromSql()
        {
            var con = new SqlConnection(HackyConString);
            con.Open();
            var cmd = con.CreateCommand();
            cmd.CommandText = "select top 1 ProductCategoryID FROM [SalesLT].[ProductCategory]";
            var dataReader = cmd.ExecuteReader();

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
            dataReader.Close();
            con.Close();
        }

        private void Execute(string url)
        {
            //Console.WriteLine(url);
            HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
            if (response.IsSuccessStatusCode)
            {
                var output = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
            }
            else
            {
                Console.WriteLine($"Error {response.StatusCode} , {response.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
            }
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
