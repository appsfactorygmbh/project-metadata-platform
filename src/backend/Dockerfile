FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY --chown=app ["ProjectMetadataPlatform.Api/ProjectMetadataPlatform.Api.csproj", "ProjectMetadataPlatform.Api/"]
COPY --chown=app ["ProjectMetadataPlatform.Application/ProjectMetadataPlatform.Application.csproj", "ProjectMetadataPlatform.Application/"]
COPY --chown=app ["ProjectMetadataPlatform.Domain/ProjectMetadataPlatform.Domain.csproj", "ProjectMetadataPlatform.Domain/"]
COPY --chown=app ["ProjectMetadataPlatform.Infrastructure/ProjectMetadataPlatform.Infrastructure.csproj", "ProjectMetadataPlatform.Infrastructure/"]
RUN mkdir "ProjectMetadataPlatform.Api/bin" -p --
RUN mkdir "ProjectMetadataPlatform.Application/bin" -p
RUN mkdir "ProjectMetadataPlatform.Domain/bin" -p
RUN mkdir "ProjectMetadataPlatform.Infrastructure/bin" -p
RUN dotnet restore "ProjectMetadataPlatform.Api/ProjectMetadataPlatform.Api.csproj"
COPY --chown=app . .
WORKDIR "/src/ProjectMetadataPlatform.Api"
RUN dotnet build "ProjectMetadataPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ProjectMetadataPlatform.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --chown=app --from=publish /app/publish .

ENTRYPOINT ["dotnet", "ProjectMetadataPlatform.Api.dll"]
