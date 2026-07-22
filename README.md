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
![Login Screen Dark](./screenshots/login-screen-dark.png)
![Login Screen Light](./screenshots/login-screen-light.png)

Project View
![Project View Dark](./screenshots/project-view-dark.png)
![Project View Light](./screenshots/project-view-light.png)

Project Information View
![Project Information View Dark](./screenshots/project-information-view-dark.png)
![Project Information View Light](./screenshots/project-information-view-light.png)
Project Information Edit View
![Project Information Edit View Dark](./screenshots/project-information-edit-view-dark.png)
![Project Information Edit View Light](./screenshots/project-information-edit-view-light.png)

Create Project View
![Create Project View Dark](./screenshots/create-project-view-dark.png)
![Create Project View Light](./screenshots/create-project-view-light.png)

Settings View - User Management
![Settings View - User Management Dark](./screenshots/user-management-view-dark.png)
![Settings View - User Management Light](./screenshots/user-management-view-light.png)

Settings View - User Creation
![Settings View - User Creation Dark](./screenshots/user-creation-view-dark.png)
![Settings View - User Creation Light](./screenshots/user-creation-view-light.png)

Settings View - Team Management
![Settings View - Team Management Dark](./screenshots/team-management-view-dark.png)
![Settings View - Team Management Light](./screenshots/team-management-view-light.png)

Settings View - Team Creation
![Settings View - Team Creation Dark](./screenshots/team-creation-view-dark.png)
![Settings View - Team Creation Light](./screenshots/team-creation-view-light.png)

Settings View - Department Management
![Settings View - Department Management Dark](./screenshots/department-management-view-dark.png)
![Settings View - Department Management Light](./screenshots/department-management-view-light.png)

Settings View - Department Creation
![Settings View - Department Creation Dark](./screenshots/department-creation-view-dark.png)
![Settings View - Department Creation Light](./screenshots/department-creation-view-light.png)

Settings View - BU Management
![Settings View - BU Management Dark](./screenshots/business-unit-management-view-dark.png)
![Settings View - BU Management Light](./screenshots/business-unit-management-view-light.png)

Settings View - BU Creation
![Settings View - BU Creation Dark](./screenshots/business-unit-creation-view-dark.png)
![Settings View - BU Creation Light](./screenshots/business-unit-creation-view-light.png)

Settings View - Location Management
![Settings View - Location Management Dark](./screenshots/office-location-management-view-dark.png)
![Settings View - Location Management Light](./screenshots/office-location-management-view-light.png)

Settings View - Location Creation
![Settings View - Location Creation Dark](./screenshots/office-location-creation-view-dark.png)
![Settings View - Location Creation Light](./screenshots/office-location-creation-view-light.png)


Settings View - Company Management
![Settings View - Company Management Dark](./screenshots/company-management-view-dark.png)
![Settings View - Company Management Light](./screenshots/company-management-view-light.png)

Settings View - Company Creation
![Settings View - Company Creation Dark](./screenshots/company-creation-view-dark.png)
![Settings View - Company Creation Light](./screenshots/company-creation-view-light.png)

Settings View - API-Token Management
![Settings View - API-Token Management Dark](./screenshots/api-token-management-view-dark.png)
![Settings View - API-Token Management Light](./screenshots/api-token-management-view-light.png)

Settings View - API-Token Creation
![Settings View - API-Token Creation Dark](./screenshots/api-token-creation-view-dark.png)
![Settings View - API-Token Creation Light](./screenshots/api-token-creation-view-light.png)

Settings View - Global Plugins Management
![Settings View - Global Plugins Management Dark](./screenshots/global-plugin-view-dark.png)
![Settings View - Global Plugins Management Light](./screenshots/global-plugin-view-light.png)

Settings View - Global Plugins Creation
![Settings View - Global Plugins Creation Dark](./screenshots/global-plugin-creation-view-dark.png)
![Settings View - Global Plugins Creation Light](./screenshots/global-plugin-creation-view-light.png)

Settings View - Global Plugins Edit
![Settings View - Global Plugins Edit Dark](./screenshots/global-plugin-edit-view-dark.png)
![Settings View - Global Plugins Edit Light](./screenshots/global-plugin-edit-view-light.png)

Settings View - Global Logs
![Settings View - Global Logs Dark](./screenshots/global-logs-view-dark.png)
![Settings View - Global Logs Light](./screenshots/global-logs-view-light.png)