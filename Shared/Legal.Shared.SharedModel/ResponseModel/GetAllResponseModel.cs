using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel;

public record GetAllResponseModel<T> : IResponseModel where T : IResponseModel
{
    public IEnumerable<T>? Items { get; set; }
}