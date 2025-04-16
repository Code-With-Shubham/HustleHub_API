# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything to the container
COPY . ./

# Restore dependencies
RUN dotnet restore HustleHub_API.sln

# Build the app in Release mode
RUN dotnet publish HustleHub_API/HustleHub_API.csproj -c Release -o out

# Use the ASP.NET runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the published output from the build step
COPY --from=build /app/out ./

# Expose the port the app will run on
EXPOSE 80

# Set the entry point for the container
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]

