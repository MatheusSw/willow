FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy sln and project files for restore (improves layer caching)
COPY evaluation.sln ./evaluation.sln
COPY evaluation-api/evaluation-api.csproj ./evaluation-api/evaluation-api.csproj
COPY evaluation-infrastructure/evaluation-infrastructure.csproj ./evaluation-infrastructure/evaluation-infrastructure.csproj
COPY evaluation-application/evaluation-application.csproj ./evaluation-application/evaluation-application.csproj
COPY evaluation-domain/evaluation-domain.csproj ./evaluation-domain/evaluation-domain.csproj

# Tests
COPY evaluation-api-tests/evaluation-api-tests.csproj ./evaluation-api-tests/evaluation-api-tests.csproj
COPY evaluation-infrastructure-tests/evaluation-infrastructure-tests.csproj ./evaluation-infrastructure-tests/evaluation-infrastructure-tests.csproj
COPY evaluation-application-tests/evaluation-application-tests.csproj ./evaluation-application-tests/evaluation-application-tests.csproj
COPY evaluation-domain-tests/evaluation-domain-tests.csproj ./evaluation-domain-tests/evaluation-domain-tests.csproj

RUN dotnet restore ./evaluation.sln

# Copy the rest of the source
COPY . .

RUN dotnet publish ./evaluation-api/evaluation-api.csproj -c Release -o /app/publish /p:UseAppHost=false

# ASPNET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "evaluation-api.dll"]
