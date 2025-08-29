using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.ParameterModel;

public record DeleteParameterModel(string Id, string LastModifiedById) : IParameterModel<IValidator>
{
    public IValidator Validator => new BlankValidator();
    public ApplicationHelper.ModuleName ModuleName { get; set; }
}