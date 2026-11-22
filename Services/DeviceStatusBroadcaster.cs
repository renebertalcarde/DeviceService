
using DeviceService.Hubs;
using DeviceService.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DeviceService.Services;

public sealed class DeviceStatusBroadcaster : IHostedService
{
    private readonly IDeviceUtil _deviceManager;
    private readonly IHubContext<DeviceHub> _hubContext;
    private readonly ILogger<DeviceStatusBroadcaster> _logger;

    public DeviceStatusBroadcaster(
        IDeviceUtil deviceManager,
        IHubContext<DeviceHub> hubContext,
        ILogger<DeviceStatusBroadcaster> logger)
    {
        _deviceManager = deviceManager;
        _hubContext = hubContext;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _deviceManager.StatusChanged += OnStatusChanged;
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _deviceManager.StatusChanged -= OnStatusChanged;
        return Task.CompletedTask;
    }

    private void OnStatusChanged(
        object? sender,
        DeviceStatusChangedEventArgs e)
    {
        _ = BroadcastStatusAsync(e);
    }

    private async Task BroadcastStatusAsync(
        DeviceStatusChangedEventArgs e)
    {
        try
        {
            await _hubContext.Clients
                .Group(DeviceHub.GroupName(e.DeviceId))
                .SendAsync(
                    "DeviceStatusChanged",
                    new
                    {
                        DeviceId = e.DeviceId,
                        IsConnected = e.IsConnected,
                        Error = e.Error
                    });
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Failed to broadcast status for device {DeviceId}",
                e.DeviceId);
        }
    }
}
