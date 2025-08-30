using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel;

namespace Legal.Application.Admin.CommandHandlers.Contract;

public class DeleteContractCommandHandler : ACommandHandler<IdParameterModel, EmptyResponseModel>
{
    public DeleteContractCommandHandler(
        ILogger<DeleteContractCommandHandler> logger,
        RequestHandler requestHandler,
        IContractService contractService) : base(logger, requestHandler)
    {
        Logger = logger;
        ContractService = contractService;
    }

    public ILogger<DeleteContractCommandHandler> Logger { get; }

    public IContractService ContractService { get; }

    public override async Task<EmptyResponseModel> Execute([NotNull] IdParameterModel parameter, CancellationToken cancellationToken)
    {
        var result = await ContractService.Delete(MapperHelper.Map<ContractDto>(parameter), cancellationToken);
        return new EmptyResponseModel();
    }
}
