# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything into the container (including all class libraries and solution files)
COPY . ./

# Restore NuGet packages using the main API project
RUN dotnet restore API/HustleHub_API.csproj

# Publish the API project (which includes the class libraries)
RUN dotnet publish API/HustleHub_API.csproj -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copy published output from the build stage
COPY --from=build /app/publish .

# Expose port 80
EXPOSE 80

# Set the entry point to run the API
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
