using AuctionsMonitor.Service;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AuctionsMonitor
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);
            builder.Services.AddHostedService<Monitor7788Service>();

            using var host = builder.Build();
            await host.RunAsync();
        }
    }
}
