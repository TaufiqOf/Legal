using Legal.Application.Admin.Dtos;

namespace Legal.Application.Admin.Infrastructure;

public interface IContractService
{
    Task<ContractDto> Save(ContractDto contract, CancellationToken cancellationToken);
 
    Task<bool> Delete(ContractDto contract, CancellationToken cancellationToken);

    Task<IEnumerable<ContractDto>> GetAll(CancellationToken cancellationToken);
    
    Task<ContractDto> Get(string id, CancellationToken cancellationToken);

    Task<(IEnumerable<ContractDto>, int)> Paged(int pageNumber,int pageSize, CancellationToken cancellationToken);

}