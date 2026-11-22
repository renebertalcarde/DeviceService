
using DeviceService.Models;
using DeviceService.Services;
using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace DeviceService.Hubs;

public sealed class DeviceHub : Hub
{
    private readonly IDeviceUtil _deviceManager;

    public DeviceHub(IDeviceUtil deviceManager)
    {
        _deviceManager = deviceManager;
    }

    public static string GroupName(int deviceId)
        => $"device-{deviceId}";

    // Client subscribes to a particular device.
    public async Task SubscribeToDevice(int deviceId)
    {
        if (deviceId <= 0)
            throw new HubException("Invalid device ID.");

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            GroupName(deviceId));
    }

    // Client stops monitoring a device.
    public async Task UnsubscribeFromDevice(int deviceId)
    {
        await Groups.RemoveFromGroupAsync(
            Context.ConnectionId,
            GroupName(deviceId));
    }

    // Request a device connection.
    public async Task<bool> ConnectDevice(int deviceId)
    {
        return await _deviceManager.ConnectAsync(deviceId);
    }

    // Request a device disconnection.
    public async Task DisconnectDevice(int deviceId)
    {
        await _deviceManager.DisconnectAsync(deviceId);
    }

    public override async Task OnDisconnectedAsync(
        Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
    }
}
