# Appsfactory "Metadata Platform" Backend

## Overview

This project is an ASP.NET Core application using Entity Framework Core and PostgreSQL. It provides a RESTful API for managing metadata of projects.

## Getting Started

### Prerequisites

-   [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Installation

1. Clone the repository:

    ```sh
    git clone <repository-url>
    cd backend
    ```

2. Restore the dependencies:
    ```sh
    dotnet restore
    ```

### Scripts

#### Build:

```sh
dotnet build
```

Builds the app.

#### Run:

Running the app locally requires a PostgreSQL database. The repository already contains a corresponding docker compose file.
First, install Docker and Docker Compose: https://docs.docker.com/get-docker/ and https://docs.docker.com/compose/install/.
Then, run the following command inside the repository directory to start the database container:

```sh
docker compose -f docker-compose-database.yml up --remove-orphans -d
```

Next, open a terminal in the ProjectMetadataPlatform.Api directory and run the following command to apply any existing migrations to the database:

With powershell (You may have to run `dotnet tool update --global PowerShell` first):

```pwsh
pwsh .\dotnet_ef.ps1 database update
```

With bash:

````sh
 sh ./dotnet_ef.sh database update
 ```

You can now run the app with the following command or an IDE of your choice:

```sh
dotnet run
````

#### Test:

```sh
dotnet test
```

Runs unit tests with NUnit.

### Project Structure

The project is build following the Clean Architecture principles. The project is structured as follows:

-   `ProjectMetadataPlatform.Application`: Application layer
-   `ProjectMetadataPlatform.Domain`: Domain layer
-   `ProjectMetadataPlatform.Infrastructure`: Infrastructure layer
-   `ProjectMetadataPlatform.Api`: Api/Presentation layer

-   `tests/ProjectMetadataPlatform.Application.Tests`: Application layer tests
-   `tests/ProjectMetadataPlatform.Domain.Tests`: Domain layer tests
-   `tests/ProjectMetadataPlatform.Infrastructure.Tests`: Infrastructure layer tests
-   `tests/ProjectMetadataPlatform.Api.Tests`: Api/Presentation layer tests

## Development

### Running the application

See the [Run-Script](#run) section for how to run the application with a local database.

### Database Migrations

When changing the domain models or their configurations in the infrastructure layer, you need to create a new migration.

1. Create a local database container according to the instructions in the [Run-Script](#run) section.
2. Open a terminal in the `ProjectMetadataPlatform.Api` directory.
3. Run the following command to apply the existing migrations to the database:

    With powershell:

    ```pwsh
    pwsh .\dotnet_ef.ps1 database update
    ```

    With bash:

    ```sh
     sh ./dotnet_ef.sh database update
    ```

4. Make the required changes to the domain models or their configurations.
5. Run the following command to create a new migration:

    With powershell:

    ```pwsh
    pwsh .\dotnet_ef.ps1 migrations add <migration-name>
    ```

    With bash:

    ```sh
     sh ./dotnet_ef.sh migrations add <migration-name>
    ```

6. Commit the generated migration files. The files can be found in the `ProjectMetadataPlatform.Infrastructure/Migrations` directory.
7. Push the changes to github and create a merge request.
8. Run the following command to create the migration script, then add it to the merge request description:

    With powershell:

    ```pwsh
    pwsh .\dotnet_ef.ps1 migrations script <name-of-the-last-migration>
    ```

    With bash:

    ```sh
     sh ./dotnet_ef.sh migrations script <name-of-the-last-migration>
    ```

9. Run the migration script on the database once the merge request is approved and merged onto main.
