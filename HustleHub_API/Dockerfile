# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything to the container (including folders like BusinessLogic and DataContext)
COPY . ./

# Restore dependencies
RUN dotnet restore HustleHub_API.sln

# Publish the app
RUN dotnet publish HustleHub_API.sln -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

EXPOSE 80
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
