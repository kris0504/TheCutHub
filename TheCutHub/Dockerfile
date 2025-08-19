# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore "./TheCutHub/TheCutHub.csproj"
RUN dotnet publish "./TheCutHub/TheCutHub.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
RUN mkdir -p /data
ENV ASPNETCORE_ENVIRONMENT=Production
COPY --from=build /app/publish .
CMD bash -lc 'export ASPNETCORE_URLS=http://0.0.0.0:${PORT:-8080}; dotnet TheCutHub.dll'
