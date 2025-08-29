using Legal.Service.Infrastructure.Interface;
using System.Net.Mime;

namespace Legal.Shared.SharedModel.ResponseModel;

public record FileResponseModel : IResponseModel
{
    public string ContentType { get; set; } = MediaTypeNames.Application.Octet;
    public string FileName { get; set; } = null!;
    public Stream FileStream { get; set; } = null!;
}