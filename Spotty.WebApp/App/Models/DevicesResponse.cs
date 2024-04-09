
namespace Spotty.WebApp.App.Models;

public class DevicesResponse
{
    public Device[] devices { get; set; } = [];

    public string GetCurrentDeviceId()
    {
        if (devices.Length == 0)
        {
            throw new SpotifyException("No devices found");
        }

        var activeDevice = devices.SingleOrDefault(device => device.is_active);
        if (activeDevice != null)
        {
            return activeDevice.id;
        }

        throw new SpotifyException("No active device found");
    }
}


