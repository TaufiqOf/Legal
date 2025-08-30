using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Model;
using Legal.Service.Infrastructure.Services;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel;
using Legal.Shared.SharedModel.ResponseModel.Contract;

namespace Legal.Application.Admin.QueryHandlers;

[TokenAuthorize]
public class GetByPagedContractQueryHandler : AQueryHandler<GetItemsParameterModel, PagedResponseModel<ContractResponseModel>>
{
    public GetByPagedContractQueryHandler(
        ILogger<GetByPagedContractQueryHandler> logger,
        RequestHandler requestHandler,
        IContractService contractService)
        : base(logger, requestHandler)
    {
        Logger = logger;
        ContractService = contractService;
    }

    public ILogger<GetByPagedContractQueryHandler> Logger { get; }

    public IContractService ContractService { get; }

    public override async Task<PagedResponseModel<ContractResponseModel>> Execute(GetItemsParameterModel parameter, CancellationToken cancellationToken)
    {
        var result = await ContractService.Paged(parameter.PageNumber, parameter.PageSize, cancellationToken);
        var responseModels = MapperHelper.Map<IEnumerable<ContractResponseModel>>(result.Item1);
        var totalCount = result.Item2;
        var pagedResponse = new PagedResponseModel<ContractResponseModel>();
        pagedResponse.Add(responseModels, totalCount, parameter.PageNumber, parameter.PageSize);
        return pagedResponse;
    }
}