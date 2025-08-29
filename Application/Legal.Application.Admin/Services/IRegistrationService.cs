using Legal.Application.Admin.Dtos;

namespace Legal.Application.Admin.Services;

public interface IRegistrationService
{
    Task<UserDto> RegisterUserAsync(UserDto userDto, CancellationToken cancellationToken);
}