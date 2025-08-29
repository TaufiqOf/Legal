using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.ParameterModel;

public abstract record BaseParameterModel : IParameterModel<IValidator>
{
    public virtual ApplicationHelper.ModuleName ModuleName { get; set; }
    public virtual IValidator Validator => new BlankValidator();
}