FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY DigitalShield.API.csproj ./
RUN dotnet restore DigitalShield.API.csproj

COPY . .
RUN dotnet publish DigitalShield.API.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 10000
ENTRYPOINT ["sh", "-c", "dotnet DigitalShield.API.dll --urls http://0.0.0.0:${PORT:-10000}"]
