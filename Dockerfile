# Brug en Ubuntu-baseret container med .NET 8.0 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 7144

# 🔹 Byggefasen - Brug .NET SDK til at bygge applikationen
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Kopier projektfil og gendan afhængigheder
COPY ["TravelBridgeAPI.csproj", "./"]
RUN dotnet restore "TravelBridgeAPI.csproj"

# Kopier resten af koden og byg applikationen
COPY . .
RUN dotnet build -c Release -o /app/build

# 🔹 Publiceringsfasen
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

# 🔹 Endelig runtime container baseret på .NET Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# 🔹 Exponer den port, som API'en skal køre på
EXPOSE 7144

# 🔹 Start applikationen og bind den til den ønskede port
ENTRYPOINT ["dotnet", "TravelBridgeAPI.dll", "--urls", "http://0.0.0.0:7144"]
