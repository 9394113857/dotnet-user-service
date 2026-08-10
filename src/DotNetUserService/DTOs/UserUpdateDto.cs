namespace DotNetUserService.DTOs;

public class UserUpdateDto
{
    public required string Username { get; set; }

    public required string Email { get; set; }
}