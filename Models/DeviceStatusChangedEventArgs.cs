namespace DeviceService.Models;

public class DeviceStatusChangedEventArgs : EventArgs
{
    public int DeviceId { get; init; }

    public bool IsConnected { get; init; }

    public string? Error { get; init; }
}