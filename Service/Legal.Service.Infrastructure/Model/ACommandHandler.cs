using FluentValidation;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics.CodeAnalysis;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Service.Infrastructure.Model;

public abstract class ACommandHandler<T, R> : IHandler<T, R>, IRequestHandler
    where T : IParameterModel<IValidator>
    where R : IResponseModel
{
    private readonly RequestHandler _requestHandler;

    protected ACommandHandler(ILogger logger, RequestHandler requestHandler)
    {
        Logger = logger;
        CommandName = GetType().Name;
        _requestHandler = requestHandler;
    }

    public MapperHelper MapperHelper { get; set; }

    public string CommandName { get; }

    public ILogger Logger { get; }

    public abstract Task<R> Execute([NotNull] T parameter, CancellationToken cancellationToken);

    protected async Task<ResultModel<X>> CallData<X>(ModuleName moduleName, dynamic commandModel, CancellationToken cancellationToken) where X : IResponseModel
    {
        var data = await _requestHandler.ExecuteString(moduleName.ToString(), JsonConvert.SerializeObject(commandModel), cancellationToken);
        return JsonConvert.DeserializeObject<ResultModel<X>>(JsonConvert.SerializeObject(data));
    }

    public async Task<ResultModel<IResponseModel>> ExecuteHandler(RequestModel commandModel, IParameterModel<IValidator> parameter, CancellationToken cancellationToken)
    {
        if (commandModel is not null && parameter is not null)
        {
            try
            {
                MapperHelper = MapperHelper.Instance(parameter.ModuleName);
                var started = DateTimeOffset.UtcNow;
                Logger.LogInformation($"{CommandName} Started Executing {DateTimeOffset.UtcNow}");
                var rawResult = await Execute((T)parameter, cancellationToken);
                Logger.LogInformation($"{CommandName} Finished Executing {DateTimeOffset.UtcNow} - {(DateTimeOffset.UtcNow - started).TotalMilliseconds}ms");
                return new ResultModel<IResponseModel>
                {
                    CommandId = commandModel.RequestId,
                    ReceivedDateTime = commandModel.ReceivedDateTime,
                    ResponseDateTime = DateTimeOffset.UtcNow,
                    RequestName = commandModel.RequestName,
                    ResponseTimeSpan = DateTimeOffset.UtcNow - commandModel.ReceivedDateTime,
                    Result = rawResult,
                    Success = true,
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, ex.Message);
                return new ResultModel<IResponseModel>
                {
                    CommandId = commandModel.RequestId,
                    ReceivedDateTime = commandModel.ReceivedDateTime,
                    ResponseDateTime = DateTimeOffset.UtcNow,
                    RequestName = commandModel.RequestName,
                    Success = false,
                    Error = ex.Message
                };
            }
        }
        else
        {
            Logger.LogError($"{CommandName} Error Executing Invalid Command or Paramater");
            return new ResultModel<IResponseModel>
            {
                CommandId = "Invalid",
                ReceivedDateTime = DateTimeOffset.UtcNow,
                ResponseDateTime = DateTimeOffset.UtcNow,
                RequestName = "Invalid",
                Success = false,
                Error = "Invalid Command or Paramater"
            };
        }
    }
}