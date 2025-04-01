# Brug en Ubuntu-baseret container med .NET 8.0 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Byggefasen - Brug .NET SDK til at bygge applikationen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopier projektfil og gendan afhængigheder
COPY ["TravelBridgeAPI/TravelBridgeAPI.csproj", "TravelBridgeAPI/"]
RUN dotnet restore "TravelBridgeAPI/TravelBridgeAPI.csproj"

# Kopier resten af koden og byg applikationen
COPY . .
WORKDIR "/src/TravelBridgeAPI"
RUN dotnet build --no-restore -c Release -o /app/build

# Publiceringsfasen
FROM build AS publish
RUN dotnet publish --no-build -c Release -o /app/publish

# Endelig container baseret på Ubuntu
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Start applikationen
ENTRYPOINT ["dotnet", "TravelBridgeAPI.dll"]
