# Use the official .NET 8 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY HustleHub_API/*.csproj ./HustleHub_API/
RUN dotnet restore

# Copy everything else and build
COPY . ./
WORKDIR /app/HustleHub_API
RUN dotnet publish -c Release -o /app/publish

# Use runtime image for the final build
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
