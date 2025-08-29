using FluentValidation;

namespace Legal.Service.Infrastructure.Interface;

public interface IHandler<T, R> : IRequestHandler
    where T : IParameterModel<IValidator>
    where R : IResponseModel
{
}