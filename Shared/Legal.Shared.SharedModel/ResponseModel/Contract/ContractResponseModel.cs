using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel.Contract;

public class ContractResponseModel : IResponseModel
{
    public string Id { get; set; } = default!;

    public string Author { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }
}