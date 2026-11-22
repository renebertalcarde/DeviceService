using DeviceService.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeviceService;

internal class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        using IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddSingleton<IDeviceManager, DeviceManager>();
            })
            .Build();
        host.Start();

        Console.WriteLine("Host started.");
    }
}
