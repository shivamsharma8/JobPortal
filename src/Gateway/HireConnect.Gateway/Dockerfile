FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5000

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Gateway/HireConnect.Gateway/HireConnect.Gateway.csproj", "src/Gateway/HireConnect.Gateway/"]
RUN dotnet restore "src/Gateway/HireConnect.Gateway/HireConnect.Gateway.csproj"

COPY . .

WORKDIR "/src/src/Gateway/HireConnect.Gateway"
RUN dotnet build "HireConnect.Gateway.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HireConnect.Gateway.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "HireConnect.Gateway.dll"]
