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

# Environment variables
ENV ASPNETCORE_URLS=http://+:8080
# Bỏ các biến không cần thiết
# ENV ASPNETCORE_HTTP_PORTS=8080
# ENV PORT=8080

EXPOSE 8080

ENTRYPOINT ["dotnet", "backend.dll"]