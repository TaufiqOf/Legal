using AutoMapper;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Model;
using UserResponseModel = Legal.Shared.SharedModel.ResponseModel.User.UserResponseModel;

namespace Legal.Application.Admin;

public static class MappingConfig
{
    public static IMapper RegisterMappings()
    {
        var mapper = MapperHelper.Instance(ApplicationHelper.ModuleName.ADMIN);

        mapper.CreateMap<User, UserResponseModel>();

        mapper.InitializeMapper();

        return mapper.GetMapper();
    }
}