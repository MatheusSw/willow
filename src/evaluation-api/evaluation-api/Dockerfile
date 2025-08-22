# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy sln and project files for restore (improves layer caching)
COPY src/evaluation-api/evaluation.sln ./src/evaluation-api/evaluation.sln
COPY src/evaluation-api/evaluation-api/evaluation-api.csproj ./src/evaluation-api/evaluation-api/evaluation-api.csproj
COPY src/evaluation-api/evaluation-infrastructure/evaluation-infrastructure.csproj ./src/evaluation-api/evaluation-infrastructure/evaluation-infrastructure.csproj
COPY src/evaluation-api/evaluation-application/evaluation-application.csproj ./src/evaluation-api/evaluation-application/evaluation-application.csproj
COPY src/evaluation-api/evaluation-domain/evaluation-domain.csproj ./src/evaluation-api/evaluation-domain/evaluation-domain.csproj

RUN dotnet restore ./src/evaluation-api/evaluation.sln

# Copy the rest of the source
COPY src/evaluation-api/ ./src/evaluation-api/

RUN dotnet publish ./src/evaluation-api/evaluation-api/evaluation-api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ASPNET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "evaluation-api.dll"]
