using System;
using System.IO;
using System.Net.Http;

namespace HttpHandler
{
    class Program
    {
        static void Main(string[] args)
        {
            var sqlClient = new SqlClient();
            sqlClient.ReadData();
            //SignalRSqlClient signalRClient = new SignalRSqlClient();
            //signalRClient.OpenConnection().GetAwaiter().GetResult();
            Console.ReadLine();
        }


        private void TestHttp2()
        {
            Console.WriteLine("Call httpv2 option 1");
            Console.ReadLine();
            string url = "https://localhost:5001/WeatherForecast";

            using (var client = new HttpClient(new WinHttpHandler()))
            {
                client.BaseAddress = new Uri(url);

                HttpResponseMessage response = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, url)
                    {
                        Version = new Version(2, 0),
                    }).GetAwaiter().GetResult(); ;
                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(response.StatusCode);
                }
            }

            Console.WriteLine("Call httpv2 option 2");
            Console.ReadLine();
            using (var client = new HttpClient(new ForceHttp2Handler(new WinHttpHandler())))
            {
                HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(response.StatusCode);
                }
            }
            Console.WriteLine("Call httpv1");
            Console.ReadLine();
            using (var client = new HttpClient())
            {


                HttpResponseMessage response = client.GetAsync(url).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode)
                {
                    var output = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(response.StatusCode);
                }
            }


            Console.ReadLine();
        }
    }
}
