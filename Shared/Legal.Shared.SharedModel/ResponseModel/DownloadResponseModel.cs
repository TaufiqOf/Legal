using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel;

public record DownloadResponseModel : IResponseModel
{
    public string ContentType { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public FileStream FileStream { get; set; }
}