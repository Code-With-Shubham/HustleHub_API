FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project directories
COPY HustleHub_API.sln ./
COPY HustleHub_API/ HustleHub_API/

# Restore the dependencies for the solution
RUN dotnet restore "HustleHub_API.sln"

# Build the project
RUN dotnet build "HustleHub_API.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "HustleHub_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final image with the published app
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
