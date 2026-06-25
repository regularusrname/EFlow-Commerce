FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /source

COPY src/Payment.Worker/Payment.Worker.csproj src/Payment.Worker/
COPY src/Eflow.Contracts/Eflow.Contracts.csproj src/Eflow.Contracts/

RUN dotnet restore src/Payment.Worker/Payment.Worker.csproj

# COPY src/Payment.Worker/ Payment.Worker/
# COPY src/Eflow.Contracts/ src/Eflow.Contracts/
COPY . .

# WORKDIR /src/Payment.Worker

RUN dotnet publish src/Payment.Worker/Payment.Worker.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS runtime

WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT [ "dotnet", "Payment.Worker.dll" ]
