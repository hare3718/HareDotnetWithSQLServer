namespace hareDotnetSecondAPI.DTOs;

public class UserForRegistrationDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Gender { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public string PasswordConfirm { get; set; } = null!;
}
