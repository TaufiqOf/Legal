using FluentValidation;
using Legal.Shared.SharedModel.Validator;

namespace Legal.Shared.SharedModel.ParameterModel;

public record GetItemsParameterModel : BaseParameterModel
{
    public override IValidator Validator => new GetItemsValidator();

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}