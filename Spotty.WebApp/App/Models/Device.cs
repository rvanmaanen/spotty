
namespace Spotty.WebApp.App.Models;

public class Device
{
    public string id { get; set; } = string.Empty;
    public bool is_active { get; set; }
    public bool is_private_session { get; set; }
    public bool is_restricted { get; set; }
    public string name { get; set; } = string.Empty;
    public bool supports_volume { get; set; }
    public string type { get; set; } = string.Empty;
    public int volume_percent { get; set; }
}


