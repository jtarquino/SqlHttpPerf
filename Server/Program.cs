using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SqlDataProviderClearText;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SqlDataProviderSslHttp2
{
    public class Program
    {
        private static readonly HttpClient _client = new HttpClient()
        {
            // Allow only HTTP/2, no downgrades or upgrades.
            DefaultVersionPolicy = HttpVersionPolicy.RequestVersionExact,
            DefaultRequestVersion = HttpVersion.Version20
        };
        public static void Main(string[] args)
        {
            using var listener = new HttpEventListener();

            RunOnlyServer();

        }

        private static void RunWithClient()
        {
            var webWorker = Task.Run(() => CreateHostBuilder().Build().Run());

            // Send an HTTP request.
            using var response = _client.GetAsync("https://localhost:5001/WeatherForecast").GetAwaiter().GetResult();

            Console.ReadLine();
            webWorker.Wait();
        }

        private static void RunOnlyServer()
        {
            CreateHostBuilder().Build().Run();
        }


        public static IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                //.ConfigureLogging(logging =>
                //{
                //    logging.ClearProviders();
                //    logging.AddConsole();
                //})
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
