namespace hareDotnetSecondAPI.DTOs;

public class UserForLoginConfirmation
{
    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;
}
