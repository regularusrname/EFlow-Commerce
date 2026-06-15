FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Orders.API.csproj Orders.API/

RUN dotnet restore Orders.API/Orders.API.csproj

COPY ./ Orders.API/

WORKDIR /src/Orders.API

RUN dotnet publish Orders.API.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish ./

EXPOSE 8080

ENTRYPOINT [ "dotnet", "Orders.API.dll" ]
