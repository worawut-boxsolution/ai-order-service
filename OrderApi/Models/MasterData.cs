using System.ComponentModel.DataAnnotations;

namespace OrderApi.Models;

public enum UserRole
{
    SuperAdmin,
    Admin,
    Technician
}

public class User
{
    [Key]
    public string User_ID { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Phone { get; set; } = default!;
    public string Password { get; set; } = default!;
    public UserRole Role { get; set; }
    public string Status { get; set; } = "Active";
}

public class MasterBranch
{
    [Key]
    public string Branch_Code { get; set; } = default!;
    public string Branch_Name { get; set; } = default!;
    public string Province { get; set; } = default!;
    public string District { get; set; } = default!;
    public string Location { get; set; } = default!; // Lat, Long
}

public class MasterJobType
{
    [Key]
    public string JobType_ID { get; set; } = default!;
    public string JobType_Name { get; set; } = default!;
    public string Status { get; set; } = "Active";
}
