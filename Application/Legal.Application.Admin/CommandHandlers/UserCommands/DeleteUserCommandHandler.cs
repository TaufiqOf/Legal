using Legal.Service.Infrastructure.Exceptions;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Legal.Service.Infrastructure.Services;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel.User;

namespace Legal.Application.Admin.CommandHandlers.UserCommands;

[TokenAuthorize]
public class DeleteUserCommandHandler : ACommandHandler<GetParameterModel, UserResponseModel>
{
    private readonly IRepository<User, AdminDatabaseContext> _repository;

    public DeleteUserCommandHandler(
        ILogger<DeleteUserCommandHandler> logger,
        IRepository<User, AdminDatabaseContext> repository,
        RequestHandler requestHandler) : base(logger, requestHandler)
    {
        _repository = repository;
    }

    public override async Task<UserResponseModel> Execute(GetParameterModel parameter,
        CancellationToken cancellationToken)
    {
        var user = await _repository.Get(parameter.Id, cancellationToken);
        NotFoundException.ThrowIfNull(user, "user not found");

        await _repository.Delete(user);
        await _repository.Commit(cancellationToken);

        return MapperHelper.Map<UserResponseModel>(user);
    }
}