using DeviceService.Models;

namespace DeviceService.Data;

public class Datasource
{
    public List<Device> Devices { get; set; } = [];
    public Device GetDeviceById(int deviceId) => Devices.FirstOrDefault(x => x.Id == deviceId);
}
