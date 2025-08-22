# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY src/admin-api/admin.sln ./src/admin-api/admin.sln
COPY src/admin-api/admin-api/admin-api.csproj ./src/admin-api/admin-api/admin-api.csproj
COPY src/admin-api/admin-infrastructure/admin-infrastructure.csproj ./src/admin-api/admin-infrastructure/admin-infrastructure.csproj
COPY src/admin-api/admin-application/admin-application.csproj ./src/admin-api/admin-application/admin-application.csproj
COPY src/admin-api/admin-domain/admin-domain.csproj ./src/admin-api/admin-domain/admin-domain.csproj

RUN dotnet restore ./src/admin-api/admin.sln

COPY src/admin-api/ ./src/admin-api/

RUN dotnet publish ./src/admin-api/admin-api/admin-api.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

ENV ASPNETCORE_URLS=http://0.0.0.0:8080

EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "admin-api.dll"]


