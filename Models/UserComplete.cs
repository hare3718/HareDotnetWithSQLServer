namespace hareDotnetSecondAPI.Models;

public partial class UserComplete
{
    public int UserId { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Gender { get; set; } = null!;

    public bool Active { get; set; }

    public string JobTitle { get; set; } = null!;

    public string Department { get; set; } = null!;

    public decimal Salary { get; set; }

    public decimal AvgSalary { get; set; }
}