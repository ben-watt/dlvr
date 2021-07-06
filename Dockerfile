FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore-env
WORKDIR /app

COPY /src/core/*.csproj core/
COPY /src/interfaces/messaging-sidecar-interfaces/*.csproj interfaces/messaging-sidecar-interfaces/
COPY /src/providers/service-bus/aspnet-extensions/*.csproj providers/service-bus/aspnet-extensions/
COPY /src/providers/service-bus/plugin/*.csproj providers/service-bus/plugin/

RUN dotnet restore ./core/*.csproj

FROM restore-env AS build-env

COPY ./src/ ./
RUN dotnet publish ./core -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine-amd64
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "messaging-sidecar.dll"]