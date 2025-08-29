using Legal.Service.Infrastructure.Interface;

namespace Legal.Shared.SharedModel.ResponseModel.User;

public record UserTypeResponseModel(string Id, string Name, string NormalizedName) : IResponseModel;