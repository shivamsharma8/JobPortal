FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/JobService/JobService.Domain/JobService.Domain.csproj", "src/Services/JobService/JobService.Domain/"]
COPY ["src/Services/JobService/JobService.Application/JobService.Application.csproj", "src/Services/JobService/JobService.Application/"]
COPY ["src/Services/JobService/JobService.Infrastructure/JobService.Infrastructure.csproj", "src/Services/JobService/JobService.Infrastructure/"]
COPY ["src/Services/JobService/JobService.API/JobService.API.csproj", "src/Services/JobService/JobService.API/"]

RUN dotnet restore "src/Services/JobService/JobService.API/JobService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/JobService/JobService.API"
RUN dotnet build "JobService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "JobService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "JobService.API.dll"]
