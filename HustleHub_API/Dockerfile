FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# Copy solution file
COPY *.sln .

# Copy all project folders (just the .csproj is not enough!)
COPY HustleHub_API/ HustleHub_API/
COPY BusinessLogic/ BusinessLogic/
COPY DataContext/ DataContext/

# Restore all projects referenced in the solution
RUN dotnet restore "HustleHub_API.sln"

# Build, publish, etc.
