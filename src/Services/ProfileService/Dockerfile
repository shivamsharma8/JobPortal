FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/ProfileService/ProfileService.Domain/ProfileService.Domain.csproj", "src/Services/ProfileService/ProfileService.Domain/"]
COPY ["src/Services/ProfileService/ProfileService.Application/ProfileService.Application.csproj", "src/Services/ProfileService/ProfileService.Application/"]
COPY ["src/Services/ProfileService/ProfileService.Infrastructure/ProfileService.Infrastructure.csproj", "src/Services/ProfileService/ProfileService.Infrastructure/"]
COPY ["src/Services/ProfileService/ProfileService.API/ProfileService.API.csproj", "src/Services/ProfileService/ProfileService.API/"]

RUN dotnet restore "src/Services/ProfileService/ProfileService.API/ProfileService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/ProfileService/ProfileService.API"
RUN dotnet build "ProfileService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ProfileService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "ProfileService.API.dll"]
