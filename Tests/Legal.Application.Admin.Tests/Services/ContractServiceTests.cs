using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Models;
using Legal.Application.Admin.Services.Contract;
using Legal.Service.Infrastructure.Interface;
using Moq;
using Xunit;

namespace Legal.Application.Admin.Tests.Services;

public class ContractServiceTests
{
    private readonly Mock<IDomainRepository<Contract, AdminDatabaseContext>> _repoMock;
    private readonly IMapper _mapper;
    private readonly ContractService _service;

    public ContractServiceTests()
    {
        _repoMock = new Mock<IDomainRepository<Contract, AdminDatabaseContext>>(MockBehavior.Strict);

        var cfg = new MapperConfiguration(c =>
        {
            c.CreateMap<Contract, ContractDto>().ReverseMap();
        });
        _mapper = cfg.CreateMapper();
        _service = new ContractService(_mapper, _repoMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsMappedDto_WhenFound()
    {
        var contract = new Contract { Id = "1", Author = "A", Name = "N", Description = "D", Created = DateTime.UtcNow };
        _repoMock.Setup(r => r.Get("1", It.IsAny<CancellationToken>())).ReturnsAsync(contract);

        var result = await _service.Get("1", CancellationToken.None);

        result.Should().NotBeNull();
        result.Id.Should().Be("1");
        _repoMock.Verify(r => r.Get("1", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetAll_ReturnsMappedDtos()
    {
        var list = new List<Contract>
        {
            new() { Id = "1", Author = "A", Name = "N1", Description = "D1", Created = DateTime.UtcNow },
            new() { Id = "2", Author = "B", Name = "N2", Description = "D2", Created = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetAll(It.IsAny<CancellationToken>())).ReturnsAsync(list);

        var result = (await _service.GetAll(CancellationToken.None)).ToList();
        result.Should().HaveCount(2);
        result.Select(x => x.Id).Should().BeEquivalentTo(new[] { "1", "2" });
    }

    [Fact]
    public async Task Paged_ReturnsMappedDtosAndTotal()
    {
        var items = new List<Contract>
        {
            new() { Id = "1", Author = "A", Name = "N1", Description = "D1", Created = DateTime.UtcNow }
        };
        _repoMock.Setup(r => r.GetPagedData(1, 10, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Items: (IList<Contract>)items, Total: 5));

        var (data, total) = await _service.Paged(1, 10, CancellationToken.None);
        data.Should().HaveCount(1);
        total.Should().Be(5);
    }

    [Fact]
    public async Task Save_NewContract_AddsAndCommits()
    {
        var dto = new ContractDto { Id = "new", Author = "A", Name = "Name", Description = "Desc", Created = DateTime.UtcNow };
        _repoMock.Setup(r => r.Get("new", It.IsAny<CancellationToken>())).ReturnsAsync((Contract?)null);
        _repoMock.Setup(r => r.Add(It.IsAny<Contract>(), It.IsAny<CancellationToken>())).ReturnsAsync((Contract c, CancellationToken _) => c);
        _repoMock.Setup(r => r.Commit(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.Save(dto, CancellationToken.None);
        result.Id.Should().Be("new");
        _repoMock.Verify(r => r.Add(It.Is<Contract>(c => c.Id == "new"), It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Save_ExistingContract_UpdatesAndCommits()
    {
        var existing = new Contract { Id = "ex", Author = "A", Name = "Old", Description = "Old", Created = DateTime.UtcNow };
        var dto = new ContractDto { Id = "ex", Author = "B", Name = "New", Description = "New", Created = existing.Created };
        _repoMock.Setup(r => r.Get("ex", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.Update(It.IsAny<Contract>())).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.Commit(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.Save(dto, CancellationToken.None);
        result.Name.Should().Be("New");
        _repoMock.Verify(r => r.Update(It.Is<Contract>(c => c.Name == "New" && c.Author == "B")), Times.Once);
        _repoMock.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_Existing_DeletesAndCommits()
    {
        var existing = new Contract { Id = "d", Author = "A", Name = "Del", Description = "Del", Created = DateTime.UtcNow };
        _repoMock.Setup(r => r.Get("d", It.IsAny<CancellationToken>())).ReturnsAsync(existing);
        _repoMock.Setup(r => r.Delete(existing, true, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
        _repoMock.Setup(r => r.Commit(It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

        var result = await _service.Delete("d", CancellationToken.None);
        result.Should().BeTrue();
        _repoMock.Verify(r => r.Delete(existing, true, It.IsAny<CancellationToken>()), Times.Once);
        _repoMock.Verify(r => r.Commit(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Delete_NotFound_Throws()
    {
        _repoMock.Setup(r => r.Get("missing", It.IsAny<CancellationToken>())).ReturnsAsync((Contract?)null);
        Func<Task> act = async () => await _service.Delete("missing", CancellationToken.None);
        await act.Should().ThrowAsync<KeyNotFoundException>().WithMessage("Contract not found");
    }
}
