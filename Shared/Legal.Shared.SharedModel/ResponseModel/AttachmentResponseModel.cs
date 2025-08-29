using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel;

public record AttachmentResponseModel : IResponseModel
{
    public string Id { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public string ContentType { get; set; } = null!;
    public long FileSize { get; set; }
}