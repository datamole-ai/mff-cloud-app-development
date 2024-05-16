namespace WebAppBackend.Models;

public class Device
{
    public required string Id { get; set; }
    
    public required string Model { get; set; }
    
    public DateTimeOffset ProvisionedAt { get; set; }
}
