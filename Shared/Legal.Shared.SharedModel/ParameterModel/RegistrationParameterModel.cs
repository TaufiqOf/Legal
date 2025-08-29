using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.ParameterModel;

public class RegistrationParameterModel : IParameterModel<IValidator>
{
    public string Name { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public ApplicationHelper.ModuleName ModuleName { get; set; }

    public IValidator Validator { get; } = new BlankValidator();
}