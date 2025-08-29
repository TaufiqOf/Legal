using FluentValidation;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Interface;
using Microsoft.AspNetCore.Http;

namespace Legal.Service.Infrastructure.Model;

public record UploadFileParameterModel : IParameterModel<IValidator>
{
    public virtual IValidator Validator { get; } = new BlankValidator();
    public ApplicationHelper.ModuleName ModuleName { get; set; }

    public IFormCollection FormCollection { get; set; }
}