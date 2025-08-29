using FluentValidation;
using Legal.Shared.SharedModel.ParameterModel;

namespace Legal.Shared.SharedModel.Validator;

public class GetItemsValidator : AbstractValidator<GetItemsParameterModel>
{
    public GetItemsValidator()
    {
        RuleFor(q => q.PageNumber).GreaterThanOrEqualTo(1);
        RuleFor(q => q.PageSize).GreaterThanOrEqualTo(1);
    }
}