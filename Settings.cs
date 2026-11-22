using DeviceService.Data;
using DeviceService.Models;

namespace DeviceService;

internal class Settings
{
    public static string HostIP = "192.168.1.7";
    public static Datasource MockData_Datasource = new Datasource
    {
        Devices = [
             new Device
             {
                 Id = 1,
                 Name = "Device 1",
                 IPAddress = "192.168.1.201",
                 Port = 4370,
                 CommunicationKey = 0
             }]
    };
}
