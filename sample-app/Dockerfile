FROM mcr.microsoft.com/dotnet/sdk:5.0 AS restore-env
WORKDIR /app

COPY *.csproj /app

RUN dotnet restore .

FROM restore-env AS build-env

COPY . ./
RUN dotnet publish ./ -c Release -o out


FROM mcr.microsoft.com/dotnet/aspnet:5.0-alpine-amd64
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "sample-app.dll"]