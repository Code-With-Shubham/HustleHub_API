# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy solution and project files
COPY HustleHub_API.sln ./
COPY HustleHub_API/ ./HustleHub_API/

# Restore dependencies
RUN dotnet restore "HustleHub_API.sln"

# Build the application
RUN dotnet build "HustleHub_API/HustleHub_API.csproj" -c Release -o /app/build

# Publish the application
RUN dotnet publish "HustleHub_API/HustleHub_API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Expose ports
EXPOSE 8080
EXPOSE 8081

# Copy published output
COPY --from=build /app/publish .

# Start the application
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
