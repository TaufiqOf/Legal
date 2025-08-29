namespace Legal.Application.Admin.Dtos;

public class UserDto
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string? Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsSystemAdmin { get; set; }

    public bool IsLocked { get; set; }
}