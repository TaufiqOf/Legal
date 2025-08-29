# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Directory.Build.props", "."]
COPY ["Api/Legal.Api.WebApi/Legal.Api.WebApi.csproj", "Api/Legal.Api.WebApi/"]
COPY ["Application/Legal.Application.Admin/Legal.Application.Admin.csproj", "Application/Legal.Application.Admin/"]
COPY ["Service/Legal.Service.Infrastructure/Legal.Service.Infrastructure.csproj", "Service/Legal.Service.Infrastructure/"]
COPY ["Service/Legal.Service.Helper/Legal.Service.Helper.csproj", "Service/Legal.Service.Helper/"]
COPY ["Service/Legal.Service.Repository/Legal.Service.Repository.csproj", "Service/Legal.Service.Repository/"]
COPY ["Shared/Legal.Shared.SharedModel/Legal.Shared.SharedModel.csproj", "Shared/Legal.Shared.SharedModel/"]
RUN dotnet restore "./Api/Legal.Api.WebApi/Legal.Api.WebApi.csproj"
COPY . .
WORKDIR "/src/Api/Legal.Api.WebApi"
RUN dotnet build "./Legal.Api.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Legal.Api.WebApi.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Legal.Api.WebApi.dll"]