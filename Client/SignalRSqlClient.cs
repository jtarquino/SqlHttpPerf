using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpHandler
{
    public class SignalRSqlClient
    {
        public async Task OpenConnection()
        {
            //HubConnection hubConnection = new HubConnection("https://localhost:5001/");
            //IHubProxy hubProxy = hubConnection.CreateHubProxy("rSqlEndpoint");
            //hubConnection.Start();
            //await hubProxy.Invoke("OpenConnection");
            using (var hubConnection = new HubConnection("https://localhost:5001"))
            {
                IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("StockTickerHub");
                stockTickerHubProxy.On("marketOpened", () =>
                {
                    Console.WriteLine("Market opened");
                });
                await hubConnection.Start();
            }

            //using (var hubConnection = new HubConnection("https://localhost:5001/stocks", useDefaultUrl: false))
            //{
            //    IHubProxy stockTickerHubProxy = hubConnection.CreateHubProxy("StockTickerHub");
            //    stockTickerHubProxy.On("marketOpened", () =>
            //    {
            //        Console.WriteLine("Market opened");
            //    });
            //    await hubConnection.Start();
            //}


        }
    }
}
