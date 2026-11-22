namespace DeviceService.Services;

public interface IZKEMService
{
    Task<bool> ConnectAsync(
        string ipAddress,
        int port,
        int communicationKey);

    Task DisconnectAsync();

    //Task<DeviceInformation?> GetInformationAsync(
    //    int machineNumber);

    //Task<IReadOnlyList<AttendanceRecord>> GetAttendanceAsync(
    //    int machineNumber);

    //Task<IReadOnlyList<DeviceUser>> GetUsersAsync(
    //    int machineNumber);

    //Task<bool> SetUserAsync(
    //    int machineNumber,
    //    DeviceUser user);

    //Task<bool> RegisterEventsAsync(int machineNumber);
    ValueTask DisposeAsync();
}