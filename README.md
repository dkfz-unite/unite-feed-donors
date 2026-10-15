# Donors Data Feed Service

Shared architecture, development workflows, and agent instructions:
[UNITE project documentation](https://github.com/dkfz-unite/.github/blob/main/README.md).

## General
Donors data feed service provides the following functionality:
- [Donors data feed web API](Docs/api.md) - REST API for uploading clinical and treatment data to the portal (including input data validation).
- Donors data indexing service - background service responsible for donor-centric data index creation.

## Dependencies
- [PostgreSQL](https://github.com/dkfz-unite/unite-env/tree/main/programs/postgresql) - domain data and processing tasks.
- [MongoDB](https://github.com/dkfz-unite/unite-env/tree/main/programs/mongodb) - staged submissions awaiting review/integration.
- [Elasticsearch](https://github.com/dkfz-unite/unite-env/tree/main/programs/elasticsearch) - Elasticsearch server with indices of domain data.

### Library dependencies

The repository combines SQL writing, Web submission/review payload access, and
indexing projects. Together they use distinct NuGet packages:

- `Unite.Data.Context`: domain SQL entities, mapping, and data services.
- `Unite.Indices.Context`: Elasticsearch indexing; it depends on `Unite.Indices`.
- `Unite.Cache`: MongoDB repository support for staged submissions.
- `Unite.Essentials.Tsv`: TSV model parsing/writing.

Package placement differs between projects; inspect their `.csproj` files for
current versions.

## Access
Environment|Address|Port
-----------|-------|----
Host|http://localhost:5100|5100
Docker|http://donors.feed.unite.net|80

## Configuration
To configure the application, change environment variables in either docker or [launchSettings.json](Unite.Donors.Feed.Web/Properties/launchSettings.json) file (if running locally):

- `ASPNETCORE_ENVIRONMENT` - ASP.NET environment (`Release`).
- `UNITE_API_KEY` - Shared key for JWT signature validation; authorization uses claims and policies.
- `UNITE_ELASTIC_HOST` - Elasticsearch service host (`http://es.unite.net:9200`).
- `UNITE_ELASTIC_USER` - Elasticsearch service user.
- `UNITE_ELASTIC_PASSWORD` - Elasticsearch service password.
- `UNITE_SQL_HOST` - SQL server host (`pgsql.unite.net`).
- `UNITE_SQL_PORT` - SQL server port (`5432`).
- `UNITE_SQL_USER` - SQL server user.
- `UNITE_SQL_PASSWORD` - SQL server password.
- `UNITE_MONGO_HOST` - MongoDB host (`mongo.unite.net`).
- `UNITE_MONGO_PORT` - MongoDB port (`27017`).
- `UNITE_MONGO_USER` - MongoDB user.
- `UNITE_MONGO_PASSWORD` - MongoDB password.
- `UNITE_INDEXING_BUCKET_SIZE` - Indexing bucket size (`100`).

## Installation

### Docker Compose
The easiest way to install the application is to use docker-compose:
- Environment configuration and installation scripts: https://github.com/dkfz-unite/unite-env
- Donors data feed service configuration and installation scripts: https://github.com/dkfz-unite/unite-env/tree/main/applications/unite-feed-donors

### Docker
The image of the service is available in our [registry](https://github.com/dkfz-unite/unite-feed-donors/pkgs/container/unite-feed-donors) for the following environments:
- `linux/amd64`
- `linux/arm64`

[Dockerfile](Dockerfile) is used to build an image of the application.
To build an image run the following command:
```
docker build -t unite.donors.feed:latest .
```

All application components should run in the same docker network.
To create common docker network if not yet available run the following command:
```bash
docker network create unite
```

To run application in docker run the following command:
```bash
docker run \
--name unite.donors.feed \
--restart unless-stopped \
--net unite \
--net-alias donors.feed.unite.net \
-p 127.0.0.1:5100:80 \
-e ASPNETCORE_ENVIRONMENT=Release \
-e UNITE_API_KEY=[unite_api_key] \
-e UNITE_ELASTIC_HOST=http://es.unite.net:9200 \
-e UNITE_ELASTIC_USER=[elasticsearch_user] \
-e UNITE_ELASTIC_PASSWORD=[elasticsearch_password] \
-e UNITE_SQL_HOST=pgsql.unite.net \
-e UNITE_SQL_PORT=5432 \
-e UNITE_SQL_USER=[sql_user] \
-e UNITE_SQL_PASSWORD=[sql_password] \
-e UNITE_MONGO_HOST=mongo.unite.net \
-e UNITE_MONGO_PORT=27017 \
-e UNITE_MONGO_USER=[mongo_user] \
-e UNITE_MONGO_PASSWORD=[mongo_password] \
-e UNITE_INDEXING_BUCKET_SIZE=100 \
-d \
unite.donors.feed:latest
```
