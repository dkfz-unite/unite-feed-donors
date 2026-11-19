# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
ENV ASPNETCORE_HTTP_PORTS=80
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS restore
ARG USER
WORKDIR /src
COPY ["Unite.Donors.Indices/Unite.Donors.Indices.csproj", "Unite.Donors.Indices/"]
COPY ["Unite.Donors.Feed/Unite.Donors.Feed.csproj", "Unite.Donors.Feed/"]
COPY ["Unite.Donors.Feed.Web/Unite.Donors.Feed.Web.csproj", "Unite.Donors.Feed.Web/"]
RUN --mount=type=secret,id=github_token \
    dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p "$(cat /run/secrets/github_token)" --store-password-in-clear-text \
    && dotnet restore "Unite.Donors.Indices/Unite.Donors.Indices.csproj" \
    && dotnet restore "Unite.Donors.Feed/Unite.Donors.Feed.csproj" \
    && dotnet restore "Unite.Donors.Feed.Web/Unite.Donors.Feed.Web.csproj" \
    && dotnet nuget remove source github

FROM restore AS build
COPY . .
WORKDIR "/src/Unite.Donors.Feed.Web"
RUN dotnet build --no-restore "Unite.Donors.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Donors.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Donors.Feed.Web.dll"]
