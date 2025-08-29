using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ParameterModel;

public class BulkDeleteParameterModel : IParameterModel<IValidator>
{
    public IEnumerable<string> Ids { get; set; } = new List<string>();

    public ApplicationHelper.ModuleName ModuleName { get; set; }

    public IValidator Validator { get; }
}