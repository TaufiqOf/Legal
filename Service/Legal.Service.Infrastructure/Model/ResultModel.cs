using Legal.Service.Infrastructure.Interface;

namespace Legal.Service.Infrastructure.Model;

public class ResultModel<T> : IResultModel<T> where T : IResponseModel
{
    public string RequestId { get; set; } = default!;

    public string RequestName { get; set; } = default!;

    public string CommandId { get; set; } = default!;

    public DateTimeOffset ReceivedDateTime { get; set; }

    public DateTimeOffset ResponseDateTime { get; set; }

    public TimeSpan ResponseTimeSpan { get; set; }

    public T Result { get; set; } = default!;

    public bool Success { get; set; } = true;

    public string? Error { get; set; }
}