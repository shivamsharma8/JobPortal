FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/AuthService/AuthService.Domain/AuthService.Domain.csproj", "src/Services/AuthService/AuthService.Domain/"]
COPY ["src/Services/AuthService/AuthService.Application/AuthService.Application.csproj", "src/Services/AuthService/AuthService.Application/"]
COPY ["src/Services/AuthService/AuthService.Infrastructure/AuthService.Infrastructure.csproj", "src/Services/AuthService/AuthService.Infrastructure/"]
COPY ["src/Services/AuthService/AuthService.API/AuthService.API.csproj", "src/Services/AuthService/AuthService.API/"]

RUN dotnet restore "src/Services/AuthService/AuthService.API/AuthService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/AuthService/AuthService.API"
RUN dotnet build "AuthService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AuthService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "AuthService.API.dll"]
