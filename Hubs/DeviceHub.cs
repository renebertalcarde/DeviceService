using DeviceService.Services;
using DeviceSharedLib.Device;
using DeviceSharedLib.Models;
using DeviceSharedLib.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace DeviceService.Hubs;

public sealed class DeviceHub : Hub<IConnectionAdapter>
{
    private readonly IDeviceUtil _deviceManager;

    public DeviceHub(IDeviceUtil deviceManager)
    {
        _deviceManager = deviceManager;
    }

    public static string GroupName(int deviceId)
        => $"device-{deviceId}";

    public override async Task OnConnectedAsync()
    {
        await Clients.Caller.OnConnectedAsync(Context.ConnectionId);
    }

    // Client subscribes to a particular device.
    public async Task SubscribeToDevice(int deviceId)
    {
        if (deviceId <= 0)
            throw new HubException("Invalid device ID.");

        var groupName = GroupName(deviceId);

        await Groups.AddToGroupAsync(
            Context.ConnectionId,
            groupName);


        await Clients.Group(groupName).SendAsync(
            "DeviceStatusChanged",
            new DeviceStatus
            {
                DeviceId = deviceId,
                IsConnected = true,
                Error = (string?)null
            });
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

    public async Task<List<DeviceInfo>> GetDevices()
    {
        return _deviceManager.GetDevices();
    }
        
}
