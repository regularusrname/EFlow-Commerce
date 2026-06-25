FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/Orders.API/Orders.API.csproj src/Orders.API/
COPY src/Eflow.Contracts/Eflow.Contracts.csproj src/Eflow.Contracts/

RUN dotnet restore src/Orders.API/Orders.API.csproj

# COPY src/Orders.API/ Orders.API/
COPY . .

# WORKDIR /src/Orders.API

RUN dotnet publish src/Orders.API/Orders.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT [ "dotnet", "Orders.API.dll" ]
