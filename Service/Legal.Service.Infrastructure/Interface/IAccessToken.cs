namespace Legal.Service.Infrastructure.Interface;

public interface IAccessToken
{
    string UserId { get; set; }

    string UserName { get; set; }

    string Email { get; set; }

    string Name { get; set; }

    bool IsAdmin { get; set; }
}