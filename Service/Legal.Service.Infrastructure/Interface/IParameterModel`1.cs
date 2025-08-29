using FluentValidation;
using static Legal.Service.Helper.ApplicationHelper;

namespace Legal.Service.Infrastructure.Interface;

public interface IParameterModel<T> where T : IValidator
{
    public ModuleName ModuleName { get; set; }

    IValidator Validator { get; }
}