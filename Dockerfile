# Bruger Windows-baseret container som base
FROM mcr.microsoft.com/dotnet/aspnet:8.0-windowsservercore-ltsc2022 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Byggefasen
FROM mcr.microsoft.com/dotnet/sdk:8.0-windowsservercore-ltsc2022 AS build
WORKDIR /src
COPY ["TravelBridgeAPI/TravelBridgAPI.csproj", "TravelBridgeAPI/"]
RUN dotnet restore "TravelBridgeAPI/TravelBridgAPI.csproj"

COPY . .
WORKDIR "/src/TravelBridgeAPI"
RUN dotnet build --no-restore -c Release -o /app/build

# Publiceringsfasen
FROM build AS publish
RUN dotnet publish --no-build -c Release -o /app/publish

# Endelig container
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TravelBridgeAPI.dll"]
