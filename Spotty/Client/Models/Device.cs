#pragma warning disable CA1051 // Do not declare visible instance fields
namespace Spotty.Client.Models;

public class Device
{
    public string Id = string.Empty;
    public bool Is_Active;
    public bool Is_Private_Session;
    public bool Is_Restricted;
    public string Name = string.Empty;
    public string Type = string.Empty;
    public int Volume_Percent;
}


