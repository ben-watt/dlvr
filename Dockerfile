FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore-env
WORKDIR /app

COPY *sln .

COPY /src/core/*.csproj ./src/core/
COPY /src/interfaces/messaging-sidecar-interfaces/*.csproj ./src/interfaces/messaging-sidecar-interfaces/
COPY /src/providers/service-bus/aspnet-extensions/*.csproj ./src/providers/service-bus/aspnet-extensions/
COPY /src/providers/service-bus/plugin/*.csproj ./src/providers/service-bus/plugin/

COPY /test/component-tests/*.csproj ./test/component-tests/

RUN dotnet restore

FROM restore-env AS build-env
COPY . ./
RUN dotnet build --no-restore

FROM build-env AS test-env
RUN dotnet test --no-build --results-directory ./test-results

FROM build-env AS release-env

COPY ./src ./
RUN dotnet publish -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine-amd64
WORKDIR /app
COPY --from=release-env /app/out .
ENTRYPOINT ["dotnet", "messaging-sidecar.dll"]