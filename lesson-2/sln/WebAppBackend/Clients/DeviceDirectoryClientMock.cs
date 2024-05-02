using WebAppBackend.Models;

namespace WebAppBackend.Clients;

public class DeviceDirectoryClientMock
{
    private static readonly string[] _modelNames = ["Sorter-A1", "Sorter-ASX", "Sorter-A2", "Sorter-BX"];
    
    public async Task<Device> GetDeviceAsync(string deviceId)
    {
        await Task.Delay(100);
        
        return new()
        {
            Id = deviceId,
            Model = _modelNames[deviceId.Length % (_modelNames.Length-1)],
            ProvisionedAt = DateTimeOffset.Now.Subtract(TimeSpan.FromDays(100))
        };
    }
}
