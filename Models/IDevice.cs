namespace DeviceService.Models;

public interface IDevice
{
    int Id { get; set; }        
    string Name { get; set; }
    string IPAddress { get; set; }
    int Port { get; set; }
    string? SerialNumber { get; set; } 
    bool IsEnabled { get; set; } 
    int CommunicationKey { get; set; } 
}
