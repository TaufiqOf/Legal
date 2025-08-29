using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel.User;

public class UserResponseModel : IResponseModel
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool AsOwner { get; set; } = false;

    public string Token { get; set; } = null!;
}