using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Shared.SharedModel.ParameterModel;

public record RestoreParameterModel(string Id, string LastModifiedbyId) : IParameterModel<IValidator>
{
    public IValidator Validator => new BlankValidator();

    public ApplicationHelper.ModuleName ModuleName { get; set; }
}