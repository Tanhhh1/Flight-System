FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY API-FlightSystem.sln .

COPY API-FlightSystem/ API-FlightSystem/
COPY Application/ Application/
COPY Domain/ Domain/
COPY Infrastructure/ Infrastructure/
COPY Shared/ Shared/

RUN dotnet restore

RUN dotnet publish API-FlightSystem/API-FlightSystem.csproj -c Release -o /out

FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /out .

EXPOSE 6789
ENTRYPOINT ["dotnet", "API-FlightSystem.dll"]