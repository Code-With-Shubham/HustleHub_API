# Use the official .NET SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy csproj files and restore first
COPY HustleHub_API/HustleHub_API.csproj ./HustleHub_API/
COPY BusinessLogic/HustleHub.BusinessArea.csproj ./BusinessLogic/
COPY DataContext/HustleHub.DataContext.csproj ./DataContext/
RUN dotnet restore ./HustleHub_API/HustleHub_API.csproj

# Now copy the rest of the files
COPY . ./

# Build the API project
RUN dotnet publish HustleHub_API/HustleHub_API.csproj -c Release -o out

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/out ./
EXPOSE 80
ENTRYPOINT ["dotnet", "HustleHub_API.dll"]
