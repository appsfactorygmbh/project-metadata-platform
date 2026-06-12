# Appsfactory “Metadata Platform”

### Overview

This project is an application for the storage and management of project metadata. This repository contains both an ASP.NET Core backend providing a RESTful API and a VUE 3 web frontend.

### Project Structure
- `.github`:
  - `workflows`: GitHub Actions
- `.vscode`: Config for IDE
- `deployment`: Files needed to build and deploy the project
  - `docker`: Docker compose files and dockerfiles
- `screenshots`: Screenshots of the frontend webpage
- `src`: Source files
  - `backend`: Files of the backend project
  - `frontend`: Files of the frontend project

### Prerequisites

-   [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Node.js (v26+)
- Corepack (manages package managers like Yarn)
- Yarn
- Pre-Commit

### Installation

1. Clone the repository:

    ```sh
    git clone <repository-url>
    cd project-metadata-platform
    ```

2. Restore the backend dependencies:
    ```sh
    cd src/backend
    dotnet restore
    ```
3. Enable Corepack and install frontend dependencies:
   ```sh
   cd ../../src/frontend
   corepack enable
   yarn install
   ```
4. Add Pre-Commit Hook for making sure Commits follow Conventional Commits format:
```sh
  cd ../..
  pre-commit install --hook-type commit-msg
```
### Local Deployment
The folder deployment/docker folder contains a minimal docker compose file for local deployment for testing purposes. To build and start the application run this command in the directory:
   ```sh
   docker compose -f docker-compose-local-build.yml up --build -d
   ```
The frontend is available at `http://localhost:8090` and the Swagger UI of the backend API is available at `http://localhost:8090/swagger/index.html`.

If SSO should be enabled add the following env variables to `deployment/docker/docker-compose-local-build.yml`:
```yml
- AZURE_AUTHORITY=<Valid Authority Url>
- AZURE_BACKEND_CLIENT_ID=<Valid Webapi App Registration>
- AZURE_SCOPE=<Valid API Scope>
- AZURE_FRONTEND_CLIENT_ID=<Valid SPA App Registration>
```

### Connecting to an SCIM Provider

This project supports User Provisioning via SCIM. The following user scheme with a custom PMP extension is used:

| SCIM Attribute                                                          | PMP User Attribute | Notes                            |
|-------------------------------------------------------------------------|--------------------|----------------------------------|
| externalId                                                              | Employee Number/Id | Matching precedence: 1; required |
| userName                                                                | Email              | Matching precedence: 2; required |
| active                                                                  | IsActive           | required                         |
| addresses[type eq "work"].locality          | OfficeLocation       |  Creates new Office Location if it doesn't exist already                                | 
| urn:ietf:params:scim:schemas:extension:enterprise:2.0:User:organization | Company            | creates new Company if it doesn't exist already                                 |
| urn:ietf:params:scim:schemas:extension:pmp:User:teamSupport             | TeamSupport        | checks for existing team         |
| urn:ietf:params:scim:schemas:extension:pmp:User:team                    | Teams              | checks for existing team         |
| urn:ietf:params:scim:schemas:extension:pmp:User:jobTitles               | JobTitles          |                                  |
| urn:ietf:params:scim:schemas:extension:pmp:User:businessUnits           | BusinessUnits      |     creates new Business Units if they don't exist already                             |
| urn:ietf:params:scim:schemas:extension:pmp:User:departments             | Departments        |    creates new Departments if they don't exist already                              |

## Appsfactory “Metadata Platform” Backend
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=appsfactory_project-metadata-platform-backend&metric=alert_status&token=0c6013f6d8cd878e6d2e9736839f77872d3b5d8e)](https://sonarcloud.io/summary/new_code?id=appsfactory_project-metadata-platform-backend)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=appsfactory_project-metadata-platform-backend&metric=code_smells&token=0c6013f6d8cd878e6d2e9736839f77872d3b5d8e)](https://sonarcloud.io/summary/new_code?id=appsfactory_project-metadata-platform-backend)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=appsfactory_project-metadata-platform-backend&metric=coverage&token=0c6013f6d8cd878e6d2e9736839f77872d3b5d8e)](https://sonarcloud.io/summary/new_code?id=appsfactory_project-metadata-platform-backend)
### Overview

This project is an ASP.NET Core application using Entity Framework Core and PostgreSQL. It provides a RESTful API for managing metadata of projects.

### Scripts

##### Build:

```sh
dotnet build
```

Builds the app.

##### Run:

Running the app locally requires a PostgreSQL database. The deployment folder already contains a corresponding docker compose file.
First, install Docker and Docker Compose: https://docs.docker.com/get-docker/ and https://docs.docker.com/compose/install/.
Then, run the following command inside the folder deployment/docker to start the database container:

```sh
docker compose -f docker-compose-database.yml up --remove-orphans -d
```
When using Visual Studio Code its enough to start debugging wth the configuration `C#: PMP Backend Debug`. A pre-launch task will automatically start and post-debug task will automatically stop the database.

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

#### Project Structure

The project is build following the Clean Architecture principles. The project is structured as follows:

-   `ProjectMetadataPlatform.Application`: Application layer
-   `ProjectMetadataPlatform.Domain`: Domain layer
-   `ProjectMetadataPlatform.Infrastructure`: Infrastructure layer
-   `ProjectMetadataPlatform.Api`: Api/Presentation layer

-   `tests/ProjectMetadataPlatform.Application.Tests`: Application layer tests
-   `tests/ProjectMetadataPlatform.Domain.Tests`: Domain layer tests
-   `tests/ProjectMetadataPlatform.Infrastructure.Tests`: Infrastructure layer tests
-   `tests/ProjectMetadataPlatform.Api.Tests`: Api/Presentation layer tests

### Development

#### Running the application

See the [Run-Script](#run) section for how to run the application with a local database.

### Auth

The Application supports authentication via basic login and SSO with Microsoft Entra ID (modeled after BFF pattern). By default, the config for SSO is filled with placeholder values. When needing to debug SSO the following values in `ProjectMetadataPlatform.Api/Properties/launchsettings.json` have to be changed:
```json
        "AZURE_AUTHORITY":"<Valid Authority URI>",
        "AZURE_BACKEND_CLIENT_ID":"<Valid Client ID for WebApi App Registration>",
        "AZURE_SCOPE":"<Valid API Scope>",
        "AZURE_FRONTEND_CLIENT_ID":"<Valid Client ID for SPA App Registration>",
```

#### Database Migrations

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
7. Push the changes to GitHub and create a merge request.
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

## Appsfactory “Metadata Platform” Frontend
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=appsfactory_project-metadata-platform-frontend&metric=alert_status&token=e8a8f47687810247f3027043088407c7e0222acc)](https://sonarcloud.io/summary/new_code?id=appsfactory_project-metadata-platform-frontend)
[![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=appsfactory_project-metadata-platform-frontend&metric=code_smells&token=e8a8f47687810247f3027043088407c7e0222acc)](https://sonarcloud.io/summary/new_code?id=appsfactory_project-metadata-platform-frontend)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=appsfactory_project-metadata-platform-frontend&metric=coverage&token=e8a8f47687810247f3027043088407c7e0222acc)](https://sonarcloud.io/summary/new_code?id=appsfactory_project-metadata-platform-frontend)
### Overview

This project is a Vue 3 web application with TypeScript integration, utilizing modern tools like Vite, ESLint, Prettier, and Vitest for development and testing.

### Scripts

**Development**:

```sh
yarn dev
```

Runs the app in development mode.

**Build**:

```sh
yarn build
```

Builds the app for production.

**Preview**:

```sh
yarn preview
```

Previews the production build.

**Lint**:

```sh
yarn lint
```

Lints the codebase with ESLint. Recommended to execute before committing.

**Format**:

```sh
yarn format
```

Formats the codebase with Prettier. Recommended to execute before committing.

**Test**:

```sh
yarn test
```

Runs unit tests with Vitest.

**Test UI**:

```sh
yarn test:ui
```

Runs the Vitest UI.

### Project Structure

- `public`: Static public files like favicon or fonts
- `src`: Source files
  - `assets`: Static assets like images
  - `components`: Vue reusable components
  - `models`: TypeScript models
  - `router`: Vue router configuration
  - `services`: API services
  - `store`: Pinia stores
  - `views`: Vue views/pages


### Development

#### Debugging the application

When using Visual Studio Code you can debug the application with either Chrome or Firefox when using the debug configurations `Chrome: PMP Frontend Debug` or `Firefox: PMP Frontend Debug`. A pre-launch task will automatically run the application in development mode, so that the browser debugger can attach, and a post-debug task will cstop the application. When using a version of Chrome / Firefox that was installed via snap package, Flatpak etc. it might be necessary to alter the launch.json and add a tmp directionary for the browser to use.
Otherwise, when not using Visual Studio Code, just start the application with: 
```sh
yarn dev
```
#### Development with Backend Service

To use the backend service during development, one needs to run the backend service locally. 

1. Start the backend as described in the [Run-Script](#run) section.
2. The backend service should now be available at `http://localhost:5091`. This URL is already configured in the `.env` file of the frontend. Simply use `import.meta.env.VITE_BACKEND_URL + "/<your-endpoint>"` to access the api. For example:

   ```ts
   const response = await fetch(import.meta.env.VITE_BACKEND_URL + '/projects');
   ```

3. The env files are already configured to use the correct backend URLs in the staging and production environments. No further changes are necessary after local development is over.
4. The Swagger UI of the backend service is available at `http://localhost:5091/swagger/index.html`.
5. To stop the backend service, hit `Ctrl+C` in the terminal where the service is running or when using Visual Studio Code stop the backend debugging session.

#### Auth
The Frontend supports authentication via basic login and SSO with Microsoft Entra ID (modeled after BFF pattern). By default, the config for SSO is not set. When debugging SSO add the following env variables to `.env`:
```env
VITE_AZURE_FRONTEND_CLIENT_ID=<Valid Client ID for SPA App Registration>
VITE_AZURE_AUTHORITY=<Valid Authority URI>
VITE_AZURE_SCOPE=<Valid API SCope>
```
Auth also has to be configured in the backend as described in [Backend-Auth](#auth).

### Frontend Screenshots
Login Screen
![Login Screen](./screenshots/Screenshot-2026-06-11-at-14-01-52-Project-Metadata-Platform-Login.png)


Project View
![Project View](./screenshots/Screenshot-2026-06-11-at-14-11-53-Project-Metadata-Platform-Project-View.png)


Project View - Light Mode
![Project View - Light Mode](./screenshots/Screenshot-2026-06-11-at-14-17-41-Project-Metadata-Platform-Project-View-Light-Mode.png)


Create Project View
![Create Project View](./screenshots/Screenshot-2026-06-11-at-14-05-48-Project-Metadata-Platform-Project-Creation.png)


Project Information View

![Project Information View](./screenshots/Screenshot-2026-06-11-at-14-24-09-Project-Metadata-Platform-Project-Information-View.png)


Settings View - User Management
![Settings View - User Management](./screenshots/Screenshot-2026-06-11-at-14-32-05-Project-Metadata-Platform-User-Information.png)


Settings View - User Creation
![Settings View - User Creation](./screenshots/Screenshot-2026-06-11-at-14-29-12-Project-Metadata-Platform-User-Creation.png)


Settings View - Team Management
![Settings View - Team Management](./screenshots/Screenshot-2026-06-11-at-14-25-26-Project-Metadata-Platform-Team-Information.png)


Settings View - Team Creation
![Settings View - Team Creation](./screenshots/Screenshot-2026-06-11-at-14-27-17-Project-Metadata-Platform-Team-Creation.png)


Settings View - Department Management
![Settings View - Department Management](./screenshots/Screenshot-2026-06-11-at-14-33-55-Project-Metadata-Platform-Department-Information.png)


Settings View - Department Creation
![Settings View - Department Creation](./screenshots/Screenshot-2026-06-11-at-14-37-08-Project-Metadata-Platform-Department-Creation.png)


Settings View - BU Management
![Settings View - BU Management](./screenshots/Screenshot-2026-06-11-at-14-40-55-Project-Metadata-Platform-Business-Unit-Information.png)


Settings View - BU Creation
![Settings View - BU Creation](./screenshots/Screenshot-2026-06-11-at-14-41-40-Project-Metadata-Platform-Business-Unit-Creation.png)


Settings View - Location Management
![Settings View - Location Management](./screenshots/Screenshot-2026-06-11-at-14-47-23-Project-Metadata-Platform-Office-Location-Information.png)


Settings View - Location Creation
![Settings View - Location Creation](./screenshots/Screenshot-2026-06-11-at-14-48-07-Project-Metadata-Platform-Office-Location-Creation.png)


Settings View - Company Management
![Settings View - Company Management](./screenshots/Screenshot-2026-06-11-at-14-50-00-Project-Metadata-Platform-Company-Information.png)


Settings View - Company Creation
![Settings View - Company Creation](./screenshots/Screenshot-2026-06-11-at-14-50-00-Project-Metadata-Platform-Company-Information.png)


Settings View - API-Token Management
![Settings View - API-Token Management](./screenshots/Screenshot-2026-06-11-at-14-54-40-Project-Metadata-Platform-API-Token-Information.png)


Settings View - API-Token Creation
![Settings View - API-Token Creation](./screenshots/Screenshot-2026-06-11-at-14-52-04-Project-Metadata-Platform-API-Token-Creation.png)


Settings View - Global Plugins Management
![Settings View - Global Plugins Management](./screenshots/Screenshot-2026-06-11-at-14-55-53-Project-Metadata-Platform-Plugins.png)


Settings View - Global Plugins Creation
![Settings View - Global Plugins Creation](./screenshots/Screenshot-2026-06-11-at-14-56-06-Project-Metadata-Platform-Create-Plugin.png)


Settings View - Global Plugins Edit
![Settings View - Global Plugins Edit](./screenshots/Screenshot-2026-06-11-at-14-55-58-Project-Metadata-Platform-Edit-Plugin.png)


Settings View - Global Logs
![Settings View - Global Logs](./screenshots/Screenshot-2026-06-11-at-15-01-02-Project-Metadata-Platform-Global-Logs.png)