using AutoMapper;
using Legal.Application.Admin.Dtos;
using Legal.Application.Admin.Models;
using Legal.Service.Helper;
using Legal.Service.Infrastructure.Helper;
using Legal.Service.Infrastructure.Model;
using Legal.Shared.SharedModel.ParameterModel;
using Legal.Shared.SharedModel.ParameterModel.Contract;
using Legal.Shared.SharedModel.ResponseModel.Contract;
using UserResponseModel = Legal.Shared.SharedModel.ResponseModel.User.UserResponseModel;

namespace Legal.Application.Admin;

public static class MappingConfig
{
    public static IMapper RegisterMappings()
    {
        var mapper = MapperHelper.Instance(ApplicationHelper.ModuleName.ADMIN);

        mapper.CreateMap<User, UserResponseModel>();
        mapper.CreateMap<ContractParameterModel, ContractDto>();
        mapper.CreateMap<ContractDto, Contract>();
        mapper.CreateMap<Contract, ContractDto>();
        mapper.CreateMap<ContractDto, ContractResponseModel>();

        mapper.CreateMap<User, UserResponseModel>();
        mapper.CreateMap<User, UserDto>();
        mapper.CreateMap<UserDto, UserResponseModel>();
        mapper.CreateMap<RegistrationParameterModel, UserDto>();
        mapper.InitializeMapper();

        return mapper.GetMapper();
    }
}