using Legal.Application.Admin.Dtos;

namespace Legal.Application.Admin.Infrastructure;

public interface IRegistrationService
{
    Task<UserDto> RegisterUserAsync(UserDto userDto, CancellationToken cancellationToken);
}