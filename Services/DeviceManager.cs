using DeviceService.Models;
using System.Collections.Concurrent;

namespace DeviceService.Services;
public sealed class DeviceManager :
    IDeviceManager, IAsyncDisposable
{

    private readonly ConcurrentDictionary<int, IZKEMService>
        _connections = new();

    private readonly ConcurrentDictionary<int, SemaphoreSlim>
        _locks = new();

    public event EventHandler<DeviceStatusChangedEventArgs>?
        StatusChanged;

    //public async Task<List<Device>> GetDevicesAsync()
    //{
    //    await using var db = await _dbFactory.CreateDbContextAsync();

    //    return await db.Devices
    //        .Where(x => 
    //            !x.ManagedByKiosk && 
    //            x.IP == "192.168.1.201"
    //            )
    //        .AsNoTracking()
    //        .OrderBy(x => x.DeviceName)
    //        .ToListAsync();
    //}

    //public async Task AddDeviceAsync(Device device)
    //{
    //    await using var db = await _dbFactory.CreateDbContextAsync();

    //    db.Devices.Add(device);

    //    await db.SaveChangesAsync();
    //}

    //public async Task UpdateDeviceAsync(Device device)
    //{
    //    await using var db = await _dbFactory.CreateDbContextAsync();

    //    db.Devices.Update(device);

    //    await db.SaveChangesAsync();
    //}

    //public async Task RemoveDeviceAsync(int deviceId)
    //{
    //    await DisconnectAsync(deviceId);

    //    await using var db = await _dbFactory.CreateDbContextAsync();

    //    var device = await db.Devices.FindAsync(deviceId);

    //    if (device == null)
    //        return;

    //    db.Devices.Remove(device);

    //    await db.SaveChangesAsync();
    //}

    public bool IsConnected(int deviceId)
    {
        return _connections.ContainsKey(deviceId);
    }

    public async Task<bool> ConnectAsync(IDevice device)
    {
        var gate = _locks.GetOrAdd(
            device.Id, _ => new SemaphoreSlim(1, 1));

        await gate.WaitAsync();

        IZKEMService? service = null;

        try
        {
            if (_connections.ContainsKey(device.Id))
                return true;

            if (device == null || !device.IsEnabled)
                return false;

            service = new ZKEMService();

            bool connected = await service.ConnectAsync(
                device.IPAddress,
                device.Port,
                device.CommunicationKey);

            if (!connected)
            {
                await service.DisposeAsync();
                service = null;

                StatusChanged?.Invoke(this,
                    new DeviceStatusChangedEventArgs
                    {
                        DeviceId = device.Id,
                        IsConnected = false,
                        Error = "Unable to connect to device."
                    });

                return false;
            }

            if (!_connections.TryAdd(device.Id, service))
            {
                await service.DisposeAsync();
                service = null;

                return false;
            }

            service = null;

            StatusChanged?.Invoke(this,
                new DeviceStatusChangedEventArgs
                {
                    DeviceId = device.Id,
                    IsConnected = true
                });

            return true;
        }
        catch
        {
            if (service != null)
                await service.DisposeAsync();

            throw;
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task DisconnectAsync(int deviceId)
    {
        var gate = _locks.GetOrAdd(
            deviceId, _ => new SemaphoreSlim(1, 1));

        await gate.WaitAsync();

        try
        {
            if (!_connections.TryRemove(
                deviceId, out var service))
                return;

            await service.DisposeAsync();

            StatusChanged?.Invoke(this,
                new DeviceStatusChangedEventArgs
                {
                    DeviceId = deviceId,
                    IsConnected = false
                });
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task DisconnectAllAsync()
    {
        foreach (var deviceId in _connections.Keys)
        {
            await DisconnectAsync(deviceId);
        }
    }

    private IZKEMService GetConnection(int deviceId)
    {
        if (!_connections.TryGetValue(
            deviceId, out var service))
        {
            throw new InvalidOperationException(
                "Device is not connected.");
        }

        return service;
    }

    //public async Task<DeviceInformation?> GetInformationAsync(
    //    int deviceId)
    //{
    //    var service = GetConnection(deviceId);

    //    var device = await GetDeviceConfigurationAsync(deviceId);

    //    return await service.GetInformationAsync(
    //        device.TerminalId);
    //}

    //public async Task<IReadOnlyList<AttendanceRecord>>
    //    GetAttendanceAsync(int deviceId)
    //{
    //    var service = GetConnection(deviceId);

    //    var device = await GetDeviceConfigurationAsync(deviceId);

    //    return await service.GetAttendanceAsync(
    //        device.TerminalId);
    //}

    //public async Task<IReadOnlyList<DeviceUser>>
    //    GetUsersAsync(int deviceId)
    //{
    //    var service = GetConnection(deviceId);

    //    var device = await GetDeviceConfigurationAsync(deviceId);

    //    return await service.GetUsersAsync(
    //        device.TerminalId);
    //}

    //public async Task<bool> SetUserAsync(
    //    int deviceId, DeviceUser user)
    //{
    //    var service = GetConnection(deviceId);

    //    var device = await GetDeviceConfigurationAsync(deviceId);

    //    return await service.SetUserAsync(
    //        device.TerminalId, user);
    //}

    //private async Task<Device>
    //    GetDeviceConfigurationAsync(int deviceId)
    //{
    //    await using var db =
    //        await _dbFactory.CreateDbContextAsync();

    //    return await db.Devices
    //        .AsNoTracking()
    //        .FirstAsync(x => x.DeviceId == deviceId);
    //}

    public async ValueTask DisposeAsync()
    {
        await DisconnectAllAsync();

        foreach (var gate in _locks.Values)
            gate.Dispose();

        _locks.Clear();
    }
}