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
        private const string getRowsBatch = "https://localhost:5001/SqlClient/GetRowsBatch";
        private const string closeReader = "https://localhost:5001/SqlClient/CloseReader";
        private const string closeConnection = "https://localhost:5001/SqlClient/CloseConnection";
        private const string doNothing= "https://localhost:5001/SqlClient/DoNothing";
        

        public void ReadData()
        {
            var sw = new Stopwatch();

            client = new HttpClient();
            sw.Reset();
            sw.Start();
            DoNothingThousand();
            var result1 = $"Http1 that does nothing: {sw.ElapsedMilliseconds} milliseconds";
            

            client = new HttpClient(new ForceHttp2Handler(new WinHttpHandler()));
            sw.Reset();
            sw.Start();
            DoNothingThousand();
            var result2 = $"Http2 that does nothing: {sw.ElapsedMilliseconds} milliseconds";

            client = new HttpClient();
            sw.Reset();
            sw.Start();
            ReadDataFromHttp();
            var result3 = $"Http1: {sw.ElapsedMilliseconds} milliseconds";

            client = new HttpClient(new ForceHttp2Handler(new WinHttpHandler()));
            sw.Reset();
            sw.Start();
            ReadDataFromHttp();
            var result4 = $"Http2: {sw.ElapsedMilliseconds} milliseconds";

            sw.Reset();
            sw.Start();
            ReadDataFromSql();
            var result5= $"Sql Native: {sw.ElapsedMilliseconds} milliseconds";


            Console.WriteLine(result1);
            Console.WriteLine(result2);
            Console.WriteLine(result3);
            Console.WriteLine(result4);
            Console.WriteLine(result5);
            //client = new HttpClient();
            //sw.Reset();
            //sw.Start();
            //ReadDataFromHttpBatch();
            //Console.WriteLine($"Http1 Batch: {sw.ElapsedMilliseconds} milliseconds");
        }

        private void ReadDataFromHttp()
        {
            for (int i = 0; i < 1000; i++)
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

        private void DoNothingThousand()
        {
            for (int i = 0; i < 1000; i++)
            {
                Execute(doNothing);
                if (i % 10 == 0) { Console.WriteLine(i); }
            }
        }

        private void ReadDataFromHttpBatch()
        {
            for (int i = 0; i < 1000; i++)
            {
                Execute(getRowsBatch);
                Execute(closeReader);
                Execute(closeConnection);
                if (i % 10 == 0) { Console.WriteLine(i); }
            }
        }

        private void ReadDataFromSql()
        {
            for (int i = 0; i < 1000; i++)
            {
                ReadeDataFromSqlOnce();
                if (i % 10 == 0) { Console.WriteLine(i); }
            }
        }

        private void ReadeDataFromSqlOnce()
        {
            var con = new SqlConnection(ConString.HackyConString);
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
