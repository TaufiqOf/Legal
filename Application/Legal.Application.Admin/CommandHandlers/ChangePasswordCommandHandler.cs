using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Legal.Service.Infrastructure.Services;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel;
using Microsoft.EntityFrameworkCore;

namespace Legal.Application.Admin.CommandHandlers;

[TokenAuthorize]
public class ChangePasswordCommandHandler : ACommandHandler<ResetPasswordParameterModel, EmptyResponseModel>
{
    private readonly IRepository<User, AdminDatabaseContext> _repository;

    public ChangePasswordCommandHandler(
        ILogger<ChangePasswordCommandHandler> logger,
        IRepository<User, AdminDatabaseContext> repository,
        RequestHandler requestHandler) : base(logger, requestHandler)
    {
        _repository = repository;
    }

    public override async Task<EmptyResponseModel> Execute([NotNull] ResetPasswordParameterModel parameter,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetQueryable()
            .Where(u => u.Id == parameter.UserName) // or u.Username
            .FirstOrDefaultAsync(cancellationToken);

        if (user == null) throw new Exception($"User with phone number {parameter.UserName} not found.");

        var valid = PasswordHasher.VerifyPassword(parameter.CurrentPassword, user.Password);
        if (!valid) throw new Exception("Incorrect current password.");

        user.Password = PasswordHasher.HashPassword(parameter.NewPassword);
        await _repository.Update(user);
        await _repository.Commit(cancellationToken);

        return new EmptyResponseModel();
    }
}