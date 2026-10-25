namespace OrderWeb.Models;

public class RoomDto
{
    public Guid Id { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public string[] Facilities { get; set; } = Array.Empty<string>();
    public bool IsExecutive { get; set; }
    public string Status { get; set; } = "AVAILABLE";
}
