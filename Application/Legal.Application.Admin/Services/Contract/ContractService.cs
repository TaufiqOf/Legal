using AutoMapper;
using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Interface;
using Microsoft.EntityFrameworkCore;

namespace Legal.Application.Admin.Services.Contract;
public class ContractService : IContractService
{
    public ContractService(
        IMapper mapper,
        IDomainRepository<Models.Contract, AdminDatabaseContext> repository)
    {
        Mapper = mapper;
        Repository = repository;
    }

    public IMapper Mapper { get; }

    public IDomainRepository<Models.Contract, AdminDatabaseContext> Repository { get; }

    public async Task<bool> Delete(ContractDto contract, CancellationToken cancellationToken)
    {
        await Repository.Delete(Mapper.Map<Models.Contract>(contract), true, cancellationToken);
        return true;
    }

    public async Task<ContractDto> Get(ContractDto contract, CancellationToken cancellationToken)
    {
        var existingContract = await Repository.Get(contract.Id, cancellationToken);
        return Mapper.Map<ContractDto>(existingContract);
    }

    public async Task<IEnumerable<ContractDto>> GetAll(CancellationToken cancellationToken)
    {
        return Mapper.Map<List<ContractDto>>(await Repository.GetAll(cancellationToken));
    }

    public async Task<(IEnumerable<ContractDto>, int)> Paged(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var models = await Repository.GetPagedData(pageNumber, pageSize, false, cancellationToken);
        var (items, total) = models;
        var dtos = Mapper.Map<IEnumerable<ContractDto>>(items);
        return (dtos, total);
    }

    public async Task<ContractDto> Save(ContractDto contract, CancellationToken cancellationToken)
    {
        var existingContract = await Repository.Get(contract.Id, cancellationToken);
        Models.Contract contractModel = null;

        if (existingContract != null)
        {
            contractModel = Mapper.Map(contract, existingContract);
            await Repository.Update(contractModel);
            await Repository.Commit(cancellationToken);
            return Mapper.Map<ContractDto>(contractModel);
        }
        else
        {
            contractModel = Mapper.Map<Models.Contract>(contract);
            await Repository.Add(contractModel);
        }

        await Repository.Commit(cancellationToken);
        return Mapper.Map<ContractDto>(contractModel);
    }
}
