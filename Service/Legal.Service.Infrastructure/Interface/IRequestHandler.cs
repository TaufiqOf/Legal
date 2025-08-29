using FluentValidation;
using Legal.Service.Infrastructure.Model;

namespace Legal.Service.Infrastructure.Interface;

public interface IRequestHandler
{
    string CommandName { get; }

    Task<ResultModel<IResponseModel>> ExecuteHandler(RequestModel commandModel, IParameterModel<IValidator> parameter, CancellationToken cancellationToken = default);
}