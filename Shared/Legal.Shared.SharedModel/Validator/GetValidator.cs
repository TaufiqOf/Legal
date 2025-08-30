using FluentValidation;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ParameterModel.Contract;

namespace Legal.Shared.SharedModel.Validator;

public class GetValidator : AbstractValidator<ContractParameterModel>
{
    public GetValidator()
    {
        RuleFor(q => q.Id).NotEmpty().NotNull();
    }
}

public class ContractValidator : AbstractValidator<ContractParameterModel>
{
    public ContractValidator()
    {
        RuleFor(q => q.Id).NotEmpty().NotNull();
        RuleFor(q => q.Name).NotEmpty().NotNull().MinimumLength(3);
        RuleFor(q => q.Author).NotEmpty().NotNull().MinimumLength(2);
        RuleFor(q => q.Description).NotEmpty().NotNull().MinimumLength(10);
    }
}