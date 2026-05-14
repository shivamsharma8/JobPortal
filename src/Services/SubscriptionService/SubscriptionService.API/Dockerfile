FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/BuildingBlocks/BuildingBlocks.Common/BuildingBlocks.Common.csproj", "src/BuildingBlocks/BuildingBlocks.Common/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Security/BuildingBlocks.Security.csproj", "src/BuildingBlocks/BuildingBlocks.Security/"]
COPY ["src/BuildingBlocks/BuildingBlocks.Contracts/BuildingBlocks.Contracts.csproj", "src/BuildingBlocks/BuildingBlocks.Contracts/"]
COPY ["src/Services/SubscriptionService/SubscriptionService.Domain/SubscriptionService.Domain.csproj", "src/Services/SubscriptionService/SubscriptionService.Domain/"]
COPY ["src/Services/SubscriptionService/SubscriptionService.Application/SubscriptionService.Application.csproj", "src/Services/SubscriptionService/SubscriptionService.Application/"]
COPY ["src/Services/SubscriptionService/SubscriptionService.Infrastructure/SubscriptionService.Infrastructure.csproj", "src/Services/SubscriptionService/SubscriptionService.Infrastructure/"]
COPY ["src/Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj", "src/Services/SubscriptionService/SubscriptionService.API/"]

RUN dotnet restore "src/Services/SubscriptionService/SubscriptionService.API/SubscriptionService.API.csproj"

COPY . .

WORKDIR "/src/src/Services/SubscriptionService/SubscriptionService.API"
RUN dotnet build "SubscriptionService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SubscriptionService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "SubscriptionService.API.dll"]
