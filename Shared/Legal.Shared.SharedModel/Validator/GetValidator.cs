using FluentValidation;
using Legal.Shared.SharedModel.ParameterModel;

namespace Legal.Shared.SharedModel.Validator;

public class GetValidator : AbstractValidator<GetParameterModel>
{
    public GetValidator()
    {
        RuleFor(q => q.Id).NotEmpty().NotNull();
    }
}