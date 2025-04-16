# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory in the container
WORKDIR /app

# Copy the entire solution and all project files into the container
COPY . .

# Restore the dependencies (this will restore all projects within the solution)
RUN dotnet restore HustleHub_API.sln

# Build the application
RUN dotnet build HustleHub_API.sln -c Release -o /app/build

# Publish the app to the /app/publish folder
RUN dotnet publish HustleHub_API.sln -c Release -o /app/publish

# Use the official .NET runtime image to run the app
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Set the working directory in the container
WORKDIR /app

# Copy the published app from the build stage
COPY --from=build /app/publish .

# Expose the port the app will run on
EXPOSE 80

# Start the app
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
