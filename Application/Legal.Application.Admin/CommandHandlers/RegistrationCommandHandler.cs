using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Services;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ResponseModel.User;

namespace Legal.Application.Admin.CommandHandlers;

public class RegistrationCommandHandler : ACommandHandler<RegistrationParameterModel, UserResponseModel>
{
    private readonly IRegistrationService _registrationService;

    public RegistrationCommandHandler(
        ILogger<RegistrationCommandHandler> logger,
        IRegistrationService registrationService,
        RequestHandler requestHandler) : base(logger, requestHandler)
    {
        _registrationService = registrationService;
    }

    public override async Task<UserResponseModel> Execute([NotNull] RegistrationParameterModel parameter,
        CancellationToken cancellationToken)
    {
        var userDto = MapperHelper.Map<UserDto>(parameter);
        userDto = await _registrationService.RegisterUserAsync(userDto, cancellationToken);

        var result = MapperHelper.Map<UserResponseModel>(userDto);
        result.Token = JwtTokenGenerator.GenerateJwtToken(userDto.Username, userDto.IsSystemAdmin);

        return result;
    }
}