using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Infrastructure;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Interface;
using Legal.Service.Infrastructure.Model;
using Moq;
using Xunit;

namespace Legal.Application.Admin.Tests.Services;

public class RegistrationServiceTests
{
    private readonly Mock<IRepository<User, AdminDatabaseContext>> _repoMock;
    private readonly IMapper _mapper;

    public RegistrationServiceTests()
    {
        _repoMock = new Mock<IRepository<User, AdminDatabaseContext>>(MockBehavior.Strict);

        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<UserDto, User>().ReverseMap();
        });
        _mapper = cfg.CreateMapper();
    }

    [Fact]
    public async Task RegisterUserAsync_NewUser_PersistsAndReturnsDto()
    {
        // Arrange
        var dto = new UserDto
        {
            Id = "new-user",
            Username = "new-user",
            Name = "New User",
            Password = "PlainPassword123"
        };

        _repoMock.Setup(r => r.Get("new-user", It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User u, CancellationToken _) => u);
        _repoMock.Setup(r => r.Commit(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var service = new RegistrationService(_repoMock.Object, _mapper);

        // Act
        var result = await service.RegisterUserAsync(dto, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be(dto.Username);
        result.Password.Should().NotBe(dto.Password); // hashed
        PasswordHasher.VerifyPassword(dto.Password, result.Password).Should().BeTrue();

        _repoMock.Verify(r => r.Add(It.Is<User>(u =>
            u.Username == dto.Username &&
            u.Password != dto.Password &&
            PasswordHasher.VerifyPassword(dto.Password, u.Password)
        ), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterUserAsync_UserAlreadyExists_Throws()
    {
        // Arrange
        var existing = new User { Id = "existing", Username = "existing", Password = "hashed" };
        var dto = new UserDto { Id = "existing", Username = "existing", Password = "Secret123" };

        _repoMock.Setup(r => r.Get("existing", It.IsAny<CancellationToken>()))
            .ReturnsAsync(existing);

        var service = new RegistrationService(_repoMock.Object, _mapper);

        // Act
        var act = async () => await service.RegisterUserAsync(dto, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<Exception>()
            .WithMessage("User existing already exists");

        _repoMock.Verify(r => r.Add(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        _repoMock.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task RegisterUserAsync_NullDto_Throws()
    {
        var service = new RegistrationService(_repoMock.Object, _mapper);
        Func<Task> act = async () => await service.RegisterUserAsync(null!, CancellationToken.None);
        await act.Should().ThrowAsync<ArgumentNullException>();
    }
}
