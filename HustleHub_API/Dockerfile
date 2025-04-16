FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy all the csproj files
COPY ["HustleHub_API/HustleHub_API.csproj", "HustleHub_API/"]
COPY ["BusinessLogic/HustleHub.BusinessArea/HustleHub.BusinessArea.csproj", "BusinessLogic/HustleHub.BusinessArea/"]
COPY ["DataContext/HustleHub.DataContext/HustleHub.DataContext.csproj", "DataContext/HustleHub.DataContext/"]

# Now copy the full folder structure so restore can resolve dependencies
COPY . .

WORKDIR "/src/HustleHub_API"

# Restore dependencies
RUN dotnet restore "HustleHub_API.csproj"

# Build the project
RUN dotnet build "HustleHub_API.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "HustleHub_API.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
