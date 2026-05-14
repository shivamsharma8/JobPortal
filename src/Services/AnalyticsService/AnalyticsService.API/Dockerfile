FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/AnalyticsService/AnalyticsService.Domain/AnalyticsService.Domain.csproj", "src/Services/AnalyticsService/AnalyticsService.Domain/"]
COPY ["src/Services/AnalyticsService/AnalyticsService.Application/AnalyticsService.Application.csproj", "src/Services/AnalyticsService/AnalyticsService.Application/"]
COPY ["src/Services/AnalyticsService/AnalyticsService.Infrastructure/AnalyticsService.Infrastructure.csproj", "src/Services/AnalyticsService/AnalyticsService.Infrastructure/"]
COPY ["src/Services/AnalyticsService/AnalyticsService.API/AnalyticsService.API.csproj", "src/Services/AnalyticsService/AnalyticsService.API/"]

RUN dotnet restore "src/Services/AnalyticsService/AnalyticsService.API/AnalyticsService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/AnalyticsService/AnalyticsService.API"
RUN dotnet build "AnalyticsService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AnalyticsService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "AnalyticsService.API.dll"]
