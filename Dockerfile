FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS restore
ARG USER
ARG TOKEN
WORKDIR /src
RUN dotnet nuget add source https://nuget.pkg.github.com/dkfz-unite/index.json -n github -u ${USER} -p ${TOKEN} --store-password-in-clear-text
COPY ["Unite.Donors.Indices/Unite.Donors.Indices.csproj", "Unite.Donors.Indices/"]
COPY ["Unite.Donors.Feed/Unite.Donors.Feed.csproj", "Unite.Donors.Feed/"]
COPY ["Unite.Donors.Feed.Web/Unite.Donors.Feed.Web.csproj", "Unite.Donors.Feed.Web/"]
RUN dotnet restore "Unite.Donors.Indices/Unite.Donors.Indices.csproj"
RUN dotnet restore "Unite.Donors.Feed/Unite.Donors.Feed.csproj"
RUN dotnet restore "Unite.Donors.Feed.Web/Unite.Donors.Feed.Web.csproj"

FROM restore as build
COPY . .
WORKDIR "/src/Unite.Donors.Feed.Web"
RUN dotnet build --no-restore "Unite.Donors.Feed.Web.csproj" -c Release

FROM build AS publish
RUN dotnet publish --no-build "Unite.Donors.Feed.Web.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Unite.Donors.Feed.Web.dll"]