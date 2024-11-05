# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY *.csproj .
RUN dotnet restore

# Copy all files and build
COPY . .
RUN dotnet publish -c Release -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production
ENV PORT=8080
ENV ASPNETCORE_HTTP_PORTS=8080
ENV ASPNETCORE_HTTPS_PORTS=8443

# Disable HTTPS redirection in Production
ENV ASPNETCORE_HTTPS_REDIRECTION_ENABLED=false

EXPOSE 8080

ENTRYPOINT ["dotnet", "backend.dll"]