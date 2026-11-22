namespace DeviceService.Models;

public class Device : IDevice
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string? SerialNumber { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public int CommunicationKey { get; set; } = 0;
}
