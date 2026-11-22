namespace DeviceService.Models;

public class Device 
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string IPAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public int CommunicationKey { get; set; } = 0;
}
