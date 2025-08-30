using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel.Contract;
using Legal.Shared.SharedModel.ResponseModel.Contract;

namespace Legal.Application.Admin.CommandHandlers.Contract;
public class SaveContractCommandHandler : ACommandHandler<ContractParameterModel, ContractResponseModel>
{
    public SaveContractCommandHandler(
        ILogger<SaveContractCommandHandler> logger,
        RequestHandler requestHandler,
        IContractService contractService) : base(logger, requestHandler)
    {
        Logger = logger;
        ContractService = contractService;
    }

    public ILogger<SaveContractCommandHandler> Logger { get; }

    public IContractService ContractService { get; }

    public override async Task<ContractResponseModel> Execute([NotNull] ContractParameterModel parameter, CancellationToken cancellationToken)
    {
        var result = await ContractService.Save(MapperHelper.Map<ContractDto>(parameter), cancellationToken);
        return MapperHelper.Map<ContractResponseModel>(result);
    }
}
