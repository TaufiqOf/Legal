using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel;

public class NotificationResponseModel : IResponseModel
{
    public string Id { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Body { get; set; } = null!;

    public string? NavigateTo { get; set; }

    public string PropertyId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTimeOffset CreateTime { get; set; }
}