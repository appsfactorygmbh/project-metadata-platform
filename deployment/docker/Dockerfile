# Build stage
FROM node:20-alpine AS frontend_build

ARG BUILD_ENV=production

WORKDIR /app

RUN npm install -g corepack && corepack enable && yarn set version stable

COPY src/frontend/yarn.lock ./
COPY src/frontend/package.json ./
COPY src/frontend/.yarnrc.yml ./

RUN rm -rf node_modules && yarn install --immutable

COPY src/frontend .
RUN yarn build --mode $BUILD_ENV

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS backend_build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --chown=app ["src/backend/ProjectMetadataPlatform.Api/ProjectMetadataPlatform.Api.csproj", "ProjectMetadataPlatform.Api/"]
COPY --chown=app ["src/backend/ProjectMetadataPlatform.Application/ProjectMetadataPlatform.Application.csproj", "ProjectMetadataPlatform.Application/"]
COPY --chown=app ["src/backend/ProjectMetadataPlatform.Domain/ProjectMetadataPlatform.Domain.csproj", "ProjectMetadataPlatform.Domain/"]
COPY --chown=app ["src/backend/ProjectMetadataPlatform.Infrastructure/ProjectMetadataPlatform.Infrastructure.csproj", "ProjectMetadataPlatform.Infrastructure/"]
RUN mkdir "ProjectMetadataPlatform.Api/bin" -p --
RUN mkdir "ProjectMetadataPlatform.Application/bin" -p
RUN mkdir "ProjectMetadataPlatform.Domain/bin" -p
RUN mkdir "ProjectMetadataPlatform.Infrastructure/bin" -p
RUN dotnet restore "ProjectMetadataPlatform.Api/ProjectMetadataPlatform.Api.csproj"
COPY --chown=app src/backend .
WORKDIR "/src/ProjectMetadataPlatform.Api"
RUN dotnet build "ProjectMetadataPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM backend_build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectMetadataPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
ENV AZURE_AUTHORITY='https://placeholder.placeholder'
ENV AZURE_BACKEND_CLIENT_ID='Placeholder'
ENV AZURE_SCOPE='Placeholder'
ENV AZURE_FRONTEND_CLIENT_ID='Placeholder'
WORKDIR /app
COPY --from=frontend_build --chown=app /app/dist /app/wwwroot
COPY --chown=app --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ProjectMetadataPlatform.Api.dll"]