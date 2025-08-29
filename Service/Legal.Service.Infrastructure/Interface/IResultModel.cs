namespace Legal.Service.Infrastructure.Interface;

public interface IResultModel<T> where T : IResponseModel
{
    string CommandId { get; set; }

    string? Error { get; set; }

    DateTimeOffset ReceivedDateTime { get; set; }

    string RequestId { get; set; }

    string RequestName { get; set; }

    DateTimeOffset ResponseDateTime { get; set; }

    TimeSpan ResponseTimeSpan { get; set; }

    T Result { get; set; }

    bool Success { get; set; }
}