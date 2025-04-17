# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY HustleHub_API.sln ./
COPY HustleHub_API/ HustleHub_API/

# Restore dependencies
RUN dotnet restore "HustleHub_API.sln"

# Build the project
RUN dotnet build "HustleHub_API/HustleHub_API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "HustleHub_API/HustleHub_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HustleHub_API.dll"]