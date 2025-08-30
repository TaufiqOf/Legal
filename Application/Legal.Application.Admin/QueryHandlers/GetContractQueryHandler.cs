using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel.Contract;

namespace Legal.Application.Admin.QueryHandlers;
public class GetContractQueryHandler : AQueryHandler<IdParameterModel, ContractResponseModel>
{
    public GetContractQueryHandler(ILogger<GetContractQueryHandler> logger, RequestHandler requestHandler, IContractService contractService) : base(logger, requestHandler)
    {
        ContractService = contractService;
    }

    public IContractService ContractService { get; }

    public override async Task<ContractResponseModel> Execute(IdParameterModel parameter, CancellationToken cancellationToken)
    {
        var result = await ContractService.Get(parameter.Id, cancellationToken);
        return MapperHelper.Map<ContractResponseModel>(result);
    }
}
