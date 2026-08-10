namespace DotNetUserService.DTOs;

public class UserCreateDto
{
    public required string Username { get; set; }

    public required string Email { get; set; }
}