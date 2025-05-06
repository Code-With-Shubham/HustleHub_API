# Use base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Use SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HustleHub_API/HustleHub_API.csproj", "HustleHub_API/"]
RUN dotnet restore "HustleHub_API/HustleHub_API.csproj"
COPY . .
WORKDIR "/src/HustleHub_API"
RUN dotnet build "HustleHub_API.csproj" -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish "HustleHub_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
