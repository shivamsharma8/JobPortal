FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/InterviewService/InterviewService.Domain/InterviewService.Domain.csproj", "src/Services/InterviewService/InterviewService.Domain/"]
COPY ["src/Services/InterviewService/InterviewService.Application/InterviewService.Application.csproj", "src/Services/InterviewService/InterviewService.Application/"]
COPY ["src/Services/InterviewService/InterviewService.Infrastructure/InterviewService.Infrastructure.csproj", "src/Services/InterviewService/InterviewService.Infrastructure/"]
COPY ["src/Services/InterviewService/InterviewService.API/InterviewService.API.csproj", "src/Services/InterviewService/InterviewService.API/"]

RUN dotnet restore "src/Services/InterviewService/InterviewService.API/InterviewService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/InterviewService/InterviewService.API"
RUN dotnet build "InterviewService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "InterviewService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

HEALTHCHECK --interval=30s --timeout=10s --start-period=40s --retries=3 \
  CMD curl -f http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "InterviewService.API.dll"]
