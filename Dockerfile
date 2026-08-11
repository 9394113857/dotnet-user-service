# =========================
# BUILD STAGE
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

# Copy project file first
COPY src/DotNetUserService/DotNetUserService.csproj src/DotNetUserService/

# Restore dependencies
RUN dotnet restore src/DotNetUserService/DotNetUserService.csproj

# Copy application source
COPY src/DotNetUserService/ src/DotNetUserService/

# Publish application
RUN dotnet publish src/DotNetUserService/DotNetUserService.csproj \
    --configuration Release \
    --output /app/publish \
    --no-restore


# =========================
# RUNTIME STAGE
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

# Render uses PORT.
# Default to 10000 for local Docker testing.
ENV ASPNETCORE_HTTP_PORTS=10000

COPY --from=build /app/publish .

EXPOSE 10000

ENTRYPOINT ["dotnet", "DotNetUserService.dll"]
