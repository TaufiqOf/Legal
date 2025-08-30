using AutoMapper;
using Legal.Application.Admin.Dtos;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;

namespace Legal.Application.Admin.Infrastructure;

public class RegistrationService(
    IRepository<User, AdminDatabaseContext> repository,
    IMapper mapper
) : IRegistrationService
{
    public async Task<UserDto> RegisterUserAsync(UserDto userDto, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(userDto);

        var existing = await repository.Get(userDto.Id, cancellationToken);
        if (existing != null)
        {
            throw new Exception($"User {userDto.Id} already exists");
        }

        var hashedPassword = PasswordHasher.HashPassword(userDto.Password);
        var userToAdd = new User
        {
            Id = userDto.Username,
            Username = userDto.Username,
            Name = userDto.Name,
            Password = hashedPassword
        };

        await repository.Add(userToAdd, cancellationToken);
        await repository.Commit(cancellationToken);

        return mapper.Map<UserDto>(userToAdd);
    }
}