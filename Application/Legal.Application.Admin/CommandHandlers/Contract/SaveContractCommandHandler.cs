using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel.Contract;
using Legal.Shared.SharedModel.ResponseModel.Contract;

namespace Legal.Application.Admin.CommandHandlers.Contract;
public class SaveContractCommandHandler : ACommandHandler<ContractParameterModel, ContractResponseModel>
{
    public SaveContractCommandHandler(ILogger logger, RequestHandler requestHandler) : base(logger, requestHandler)
    {
        Logger = logger;
    }

    public ILogger Logger { get; }

    public override Task<ContractResponseModel> Execute([NotNull] ContractParameterModel parameter, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
