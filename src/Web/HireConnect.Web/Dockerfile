FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5020
ENV ASPNETCORE_URLS=http://+:5020

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["src/Web/HireConnect.Web/HireConnect.Web.csproj", "src/Web/HireConnect.Web/"]
RUN dotnet restore "src/Web/HireConnect.Web/HireConnect.Web.csproj"

COPY src/Web/HireConnect.Web/ src/Web/HireConnect.Web/
WORKDIR "/src/src/Web/HireConnect.Web"
RUN dotnet build "HireConnect.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HireConnect.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HireConnect.Web.dll"]
