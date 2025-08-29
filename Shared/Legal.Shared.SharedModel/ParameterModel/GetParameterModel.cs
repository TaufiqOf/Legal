using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Shared.SharedModel.Validator;

namespace Legal.Shared.SharedModel.ParameterModel;

public record GetParameterModel(string Id) : IParameterModel<IValidator>
{
    public IValidator Validator => new GetValidator();
    public ApplicationHelper.ModuleName ModuleName { get; set; }
}