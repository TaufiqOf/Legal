using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel.User;
using Microsoft.EntityFrameworkCore;

namespace Legal.Application.Admin.CommandHandlers;

public class LogInCommandHandler : ACommandHandler<LogInParameterModel, UserResponseModel>
{
    private readonly IRepository<User, AdminDatabaseContext> _repository;

    public LogInCommandHandler(
        ILogger<LogInCommandHandler> logger,
        IRepository<User, AdminDatabaseContext> repository,
        RequestHandler requestHandler) : base(logger, requestHandler)
    {
        _repository = repository;
    }

    public override async Task<UserResponseModel> Execute([NotNull] LogInParameterModel parameter,
        CancellationToken cancellationToken)
    {
        var user = await _repository.GetQueryable().Where(u => u.Username == parameter.UserName)
            .FirstOrDefaultAsync(cancellationToken);
        if (user is null) throw new Exception("Invalid username or password.");

        var valid = PasswordHasher.VerifyPassword(parameter.Password, user.Password);
        if (!valid) throw new Exception("Invalid username or password.");

        var result = MapperHelper.Map<UserResponseModel>(user);
        result.Token = JwtTokenGenerator.GenerateJwtToken(user.Username, user.IsSystemAdmin);
        return result;
    }
}