using DeviceService.Hubs;
using DeviceService.Services;
using DeviceSharedLib;
using DeviceSharedLib.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DeviceService;

internal class Program
{
    static async Task Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)

            // Register as a Windows Service.
            .UseWindowsService(options =>
            {
                options.ServiceName = "DeviceService";
            })

            // Configure the embedded HTTP server.
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseUrls(Settings.HostUri.ToString());

                webBuilder.ConfigureServices(services =>
                {
                    // Mock data for development only
                    services.AddSingleton<Datasource>(builder => Settings.MockData_Datasource);

                    // Existing device manager.
                    services.AddSingleton<
                        IDeviceUtil,
                        DeviceUtil>();

                    // SignalR.
                    services.AddSignalR();

                    // Device status notifications.
                    services.AddHostedService<
                        DeviceStatusBroadcaster>();
                });

                webBuilder.Configure(app =>
                {
                    app.UseRouting();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapHub<DeviceHub>(Settings.HubPath);
                    });
                });
            })

            .Build();

        await host.RunAsync();
    }
}
