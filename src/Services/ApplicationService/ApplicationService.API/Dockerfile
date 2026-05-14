FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/ApplicationService/ApplicationService.Domain/ApplicationService.Domain.csproj", "src/Services/ApplicationService/ApplicationService.Domain/"]
COPY ["src/Services/ApplicationService/ApplicationService.Application/ApplicationService.Application.csproj", "src/Services/ApplicationService/ApplicationService.Application/"]
COPY ["src/Services/ApplicationService/ApplicationService.Infrastructure/ApplicationService.Infrastructure.csproj", "src/Services/ApplicationService/ApplicationService.Infrastructure/"]
COPY ["src/Services/ApplicationService/ApplicationService.API/ApplicationService.API.csproj", "src/Services/ApplicationService/ApplicationService.API/"]

RUN dotnet restore "src/Services/ApplicationService/ApplicationService.API/ApplicationService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/ApplicationService/ApplicationService.API"
RUN dotnet build "ApplicationService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApplicationService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "ApplicationService.API.dll"]
