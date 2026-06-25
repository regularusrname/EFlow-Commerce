FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/Catalog.API/Catalog.API.csproj src/Catalog.API/

RUN dotnet restore src/Catalog.API/Catalog.API.csproj

# COPY src/ .
COPY . .

# WORKDIR /src/Catalog.API

RUN dotnet publish src/Catalog.API/Catalog.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT [ "dotnet", "Catalog.API.dll" ]
