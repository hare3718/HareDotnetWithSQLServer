namespace hareDotnetSecondAPI.Models;

public partial class Users
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public bool Active { get; set; }
}