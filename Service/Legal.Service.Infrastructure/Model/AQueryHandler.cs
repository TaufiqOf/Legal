using FluentValidation;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Interface;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Service.Infrastructure.Model;

public abstract class AQueryHandler<T, R> : IHandler<T, R>
    where T : IParameterModel<IValidator>
    where R : IResponseModel
{
    private readonly RequestHandler _requestHandler;

    protected AQueryHandler(ILogger logger, RequestHandler requestHandler)
    {
        Logger = logger;
        CommandName = GetType().Name;
        _requestHandler = requestHandler;
    }

    public MapperHelper MapperHelper { get; set; }

    public string CommandName { get; }

    public ILogger Logger { get; }

    public abstract Task<R> Execute(T parameter, CancellationToken cancellationToken);

    protected async Task<ResultModel<TResponseModel>> CallData<TResponseModel, TParameterModel>(ModuleName moduleName, string requestName, TParameterModel parameter, CancellationToken cancellationToken) where TResponseModel : IResponseModel where TParameterModel : IParameterModel<IValidator>
    {
        var requestModel = new RequestModel<TParameterModel>()
        {
            SendDateTime = DateTime.Now,
            RequestId = Ulid.NewUlid().ToString(),
            RequestName = requestName,
            Parameter = parameter
        };
        var data = await _requestHandler.ExecuteString(moduleName.ToString(), JsonConvert.SerializeObject(requestModel), cancellationToken);
        return JsonConvert.DeserializeObject<ResultModel<TResponseModel>>(JsonConvert.SerializeObject(data));
    }

    public async Task<ResultModel<IResponseModel>> ExecuteHandler(RequestModel commandModel, IParameterModel<IValidator> parameter, CancellationToken cancellationToken = default)
    {
        if (commandModel is not null && parameter is not null)
        {
            try
            {
                MapperHelper = MapperHelper.Instance(parameter.ModuleName);

                var started = DateTimeOffset.Now;
                Logger.LogInformation($"{CommandName} Started Executing {started}");
                var rawResult = await Execute((T)parameter, cancellationToken);
                Logger.LogInformation($"{CommandName} Finished Executing - {(DateTimeOffset.Now - started).TotalMilliseconds}ms");

                return new ResultModel<IResponseModel>
                {
                    CommandId = commandModel.RequestId,
                    ReceivedDateTime = commandModel.ReceivedDateTime,
                    ResponseDateTime = DateTimeOffset.UtcNow,
                    RequestName = commandModel.RequestName,
                    ResponseTimeSpan = DateTimeOffset.UtcNow - commandModel.ReceivedDateTime,
                    Result = rawResult
                };
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, $"{CommandName} Error Executing {ex.Message}");
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