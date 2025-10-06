FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Application
COPY admin.sln ./admin.sln
COPY admin-api/admin-api.csproj ./admin-api/admin-api.csproj
COPY admin-infrastructure/admin-infrastructure.csproj ./admin-infrastructure/admin-infrastructure.csproj
COPY admin-application/admin-application.csproj ./admin-application/admin-application.csproj
COPY admin-domain/admin-domain.csproj ./admin-domain/admin-domain.csproj

# Tests
COPY admin-api-tests/admin-api-tests.csproj ./admin-api-tests/admin-api-tests.csproj
COPY admin-infrastructure-tests/admin-infrastructure-tests.csproj ./admin-infrastructure-tests/admin-infrastructure-tests.csproj
COPY admin-application-tests/admin-application-tests.csproj ./admin-application-tests/admin-application-tests.csproj
COPY admin-domain-tests/admin-domain-tests.csproj ./admin-domain-tests/admin-domain-tests.csproj

RUN dotnet restore ./admin.sln

COPY . .

RUN dotnet publish ./admin-api/admin-api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "admin-api.dll"]


