using FluentValidation;
using Legal.Service.Infrastructure.Interface;

namespace Legal.Service.Infrastructure.Model;

public class RequestModel : IRequestModel
{
    public string RequestId { get; set; } = Ulid.NewUlid().ToString();

    public string RequestName { get; set; } = default!;

    public DateTimeOffset SendDateTime { get; set; } = DateTimeOffset.UtcNow;

    public DateTimeOffset ReceivedDateTime { get; set; } = DateTimeOffset.UtcNow;
}

public class RequestModel<T> : RequestModel where T : IParameterModel<IValidator>
{
    public T Parameter { get; set; } = default!;
}