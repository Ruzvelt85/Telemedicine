Telemedicine MVP backend solution


# common-solution

- [common-solution](#common-solution)
  - [Initialize repository](#initialize-repository)
  - [Start Order](#start-order)
  - [Docker CLI commands](#docker-cli-commands)
    - [Launch over docker compose](#launch-over-docker-compose)
    - [Remove all containers](#remove-all-containers)
    - [Creating images](#creating-images)
  - [Infrastructure](#infrastructure)
    - [Databases](#databases)
    - [HealthChecks](#healthchecks)
  - [Other commands](#other-commands)

## Initialize repository

To download subrepositories/submodules after `git clone` use:

```batchfile
git submodule update --init --remote --force
```

## Start Order

- Start docker-compose either from the visual studio or from the CLI (see. [Launch over docker compose](#launch-over-docker-compose))

- Start database migration, either from the studio, executable file, or from the console (see. [Migrate database](#migrate-Migration-Database))

## Docker CLI commands

The `common-solution` is root folder

### Launch over docker compose

```batchfile
docker-compose -f docker-compose.yml -f docker-compose.override.yml -p telemedicine up -d --build --force-recreate  --remove-orphans
```

### Remove all containers

```batchfile
docker rm -f $(docker ps -a -q)
```

### Creating images

```batchfile
docker build . -f .\services\src\HealthCenterStructureDomainService\HealthCenterStructureDomainService.WebApi\Dockerfile --no-cache --tag health-center-structure-domain-service-webapi
```

## Infrastructure

### Databases

For developing purposes local PostgreSQL database and PGAdmin tool are configured in docker-compose.yml (containers `database-postgres` and `database-postgres-admin`).
Database is stored locally on volumes `postgresql` and `postgresql_data`.
To access PGAdmin via your web browser, visit the URL <http://localhost:4998/> and use the `sa@sa.com` for the email address and `Qwerty123` as the password to log in.

Then click `Servers > Create > Server` to create a new server; on Connection tab use the container name (`database-postgres`) as the value of Host name/address field (<https://towardsdatascience.com/how-to-run-postgresql-and-pgadmin-using-docker-3a6a8ae918b5>)

### HealthChecks

Health checks are configured for every service (<https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/health-checks?view=aspnetcore-5.0>).

To see health status for specific service - `http://{service-uri}:{port}/health`

Health statuses of all services also can be seen through HealthCheckUI that is configured with image `xabarilcoding/healthchecksui` in `docker-compose.yml`
(see: <https://hub.docker.com/r/xabarilcoding/healthchecksui/>, samples - <https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks>).
To see health statuses of all services after launching docker-compose: <http://localhost:4999/health>

## Other commands

Install dotnet tools:

```batchfile
dotnet tool restore
```

Install dotnet-ef:

```batchfile
dotnet tool install --global dotnet-ef
```

Launch resharper inspection:

```batchfile
dotnet jb inspectcode "src\aeb.commonInfrastructure.sln" --o=inspectcode.xml
```

Additionally is possible to use `-s=WARNING` to filter issues by Severity

Transformation inspectcode.xml into inspectcode.json:

```batchfile
dotnet resharper-to-codeclimate inspectcode.xml inspectcode.json --dotnetcoresdk=5.0.100
```
