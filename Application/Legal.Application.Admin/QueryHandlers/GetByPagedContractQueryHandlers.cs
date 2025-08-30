using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel;
using Legal.Shared.SharedModel.ResponseModel.Contract;

namespace Legal.Application.Admin.QueryHandlers;
public class GetByPagedContractQueryHandlers : AQueryHandler<GetItemsParameterModel, PagedResponseModel<ContractResponseModel>>
{
    public GetByPagedContractQueryHandlers(
        ILogger<GetByPagedContractQueryHandlers> logger, 
        RequestHandler requestHandler,
        IContractService contractService) 
        : base(logger, requestHandler)
    {
        Logger = logger;
        ContractService = contractService;
    }

    public ILogger<GetByPagedContractQueryHandlers> Logger { get; }

    public IContractService ContractService { get; }

    public override async Task<PagedResponseModel<ContractResponseModel>> Execute(GetItemsParameterModel parameter, CancellationToken cancellationToken)
    {
        var result = await ContractService.Paged(parameter.PageNumber, parameter.PageSize, cancellationToken);
        return MapperHelper.Map<PagedResponseModel<ContractResponseModel>>(result);
    }
}
