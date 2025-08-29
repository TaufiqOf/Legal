namespace Legal.Service.Infrastructure.Model;

public class User : BaseModel
{
    public User()
    {
        Id = Guid.NewGuid().ToString();
    }

    public string Username { get; set; } = null!;

    public string? Name { get; set; } = null!;

    public string Password { get; set; } = null!;

    public bool IsEnabled { get; set; }

    public bool IsSystemAdmin { get; set; }

    public bool IsLocked { get; set; }
}