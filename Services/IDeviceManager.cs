using DeviceService.Models;

namespace DeviceService.Services;

public interface IDeviceManager
{
    event EventHandler<DeviceStatusChangedEventArgs>? StatusChanged;

    //Task<List<Device>> GetDevicesAsync();

    //Task AddDeviceAsync(Device device);

    //Task UpdateDeviceAsync(Device device);

    //Task RemoveDeviceAsync(int deviceId);

    Task<bool> ConnectAsync(IDevice device);

    Task DisconnectAsync(int deviceId);

    Task DisconnectAllAsync();

    bool IsConnected(int deviceId);

    //Task<DeviceInformation?> GetInformationAsync(int deviceId);

    //Task<IReadOnlyList<AttendanceRecord>> GetAttendanceAsync(int deviceId);

    //Task<IReadOnlyList<DeviceUser>> GetUsersAsync(int deviceId);

    //Task<bool> SetUserAsync(int deviceId, DeviceUser user);
}