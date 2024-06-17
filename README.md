# Appsfactory "Metadata Platform" Frontend

## Overview

This project is a Vue 3 web application with TypeScript integration, utilizing modern tools like Vite, ESLint, Prettier, and Vitest for development and testing.

## Getting Started

### Prerequisites

- Node.js (v16+)
- Corepack (manages package managers like Yarn)
- Yarn

### Installation

1. Clone the repository:

   ```sh
   git clone <repository-url>
   cd frontend
   ```

2. Enable Corepack and install dependencies:
   ```sh
   corepack enable
   yarn install
   ```

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

### Environment Variables

...

## Development with Backend Service

To use the backend service during development, one needs to run the backend service locally. This can easily be done with docker.

1. Install Docker and Docker Compose: https://docs.docker.com/get-docker/ and https://docs.docker.com/compose/install/
2. Download the latest version of `docker-compose-staging.yml` from the backend repository: https://gitlab.dit.htwk-leipzig.de/projekt2024_A_Appsfactory_Project_Metadata_Platform/backend/-/blob/develop/docker-compose-staging.yml?ref_type=heads
3. Login to the gitlab registry in docker using `docker login gitlab.dit.htwk-leipzig.de:5050` and enter your gitlab username and a personal access token as password. The personal access token needs to have the `read_registry` scope.
4. Run the backend service by running these commands in the same directory as the downloaded `docker-compose-staging.yml` file (You may need to add `-` between `docker` and `compose`):

   ```sh
   docker compose -f docker-compose-staging.yml pull
   docker compose -f docker-compose-staging.yml up --remove-orphans
   ```

5. The backend service should now be available at `http://localhost:8090`. This URL is already configured in the `.env` file of the frontend. Simply use `import.meta.env.VITE_BACKEND_URL + "/<your-endpoint>"` to axxess the api. For example:

   ```ts
   const response = await fetch(import.meta.env.VITE_BACKEND_URL + '/projects');
   ```

6. The env files are already configured to use the correct backend URLs in the staging and production environments. No further changes are necessary after local development is over.
7. The Swagger UI of the backend service is available at `http://localhost:8090/swagger/index.html`.
8. To stop the backend service, hit `Ctrl+C` in the terminal where the service is running.
