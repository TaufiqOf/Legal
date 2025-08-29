using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.ParameterModel.Contract;
public class ContractParameterModel : IParameterModel<IValidator>
{
    public ApplicationHelper.ModuleName ModuleName { get; set; }

    public IValidator Validator => new BlankValidator();
    
    public string Id { get; set; } = default!;

    public string Author { get; set; } = default!;

    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public DateTime Created { get; set; }

    public DateTime? Updated { get; set; }

}
