using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace CsharpClient
{
    public class SqlRClient
    {
        private HubConnection connection;

        public async Task ReadData()
        {
            var sw = new Stopwatch();
            sw.Reset();
            sw.Start();

            connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:5001/rSqlEndpoint")
            .Build();
            await connection.StartAsync();
            for (int i = 0; i < 100; i++)
            {
                await OpenConnection();
                await CreateCommand();
                await ExecuteReader();
                await GetRows();
                await CloseReader();
                await CloseConnection();
                if (i % 10 == 0) { Console.WriteLine(i); }
            }
            Console.WriteLine($"WebSockets: {sw.ElapsedMilliseconds} milliseconds");
        }
        public async Task OpenConnection()
        {
            await connection.InvokeAsync("OpenConnection");
        }

        public async Task CloseConnection()
        {
            await connection.InvokeAsync("CloseConnection");
        }
        public async Task CloseReader()
        {
            await connection.InvokeAsync("CloseReader");
        }
        public async Task CreateCommand()
        {
            await connection.InvokeAsync("CreateCommand");
        }
        public async Task ExecuteReader()
        {
            await connection.InvokeAsync("ExecuteReader");
        }
        public async Task<byte[]> GetRows()
        {
            return await connection.InvokeAsync<byte[]>("GetRows");
        }

    }

}
