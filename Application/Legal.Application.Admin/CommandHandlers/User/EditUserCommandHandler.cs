using Legal.Service.Infrastructure.Exceptions;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Legal.Service.Infrastructure.Services;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel.User;

namespace Legal.Application.Admin.CommandHandlers.User;

[TokenAuthorize]
public class EditUserCommandHandler : ACommandHandler<EditUserParameterModel, UserResponseModel>
{
    private readonly IRepository<Service.Infrastructure.Model.User, AdminDatabaseContext> _repository;

    public EditUserCommandHandler(
        ILogger<EditUserCommandHandler> logger,
        IRepository<Service.Infrastructure.Model.User, AdminDatabaseContext> repository,
        RequestHandler requestHandler) : base(logger, requestHandler)
    {
        _repository = repository;
    }

    public override async Task<UserResponseModel> Execute(EditUserParameterModel parameter,
        CancellationToken cancellationToken)
    {
        var user = await _repository.Get(parameter.Id, cancellationToken);
        NotFoundException.ThrowIfNull(user, "user not found");

        user.Name = parameter.Name;
        await _repository.Update(user);
        await _repository.Commit(cancellationToken);

        return MapperHelper.Map<UserResponseModel>(user);
    }
}