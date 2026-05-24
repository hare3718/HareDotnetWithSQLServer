namespace hareDotnetSecondAPI.Models;

public partial class UserJobInfo
{
    public int UserId { get; set; }

    public string JobTitle { get; set; } = null!;

    public string Department { get; set; } = null!;
}