using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.ParameterModel;

public class ResetPasswordParameterModel : IParameterModel<IValidator>
{
    public string UserName { get; set; } = null!;

    public string CurrentPassword { get; set; } = null!;

    public string NewPassword { get; set; } = null!;

    public ApplicationHelper.ModuleName ModuleName { get; set; }

    public IValidator Validator { get; } = new BlankValidator();
}