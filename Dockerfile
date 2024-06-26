ARG ASPNET_IMAGE_TAG=8.0-bookworm-slim
ARG NODEJS_IMAGE_TAG=18.20-bullseye

# Stage 1 - Build frontend assets
FROM node:${NODEJS_IMAGE_TAG} as frontend
COPY ./Dfe.PrepareTransfers.Web/ /build/
WORKDIR /build/wwwroot
RUN npm install
RUN npm run build

# Stage 2 - Build project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS publish
WORKDIR /build

COPY ./Dfe.PrepareTransfers.Data/ ./Dfe.PrepareTransfers.Data/
COPY ./Dfe.PrepareTransfers.Data.TRAMS/ ./Dfe.PrepareTransfers.Data.TRAMS/
COPY ./Dfe.PrepareTransfers.DocumentGeneration/ ./Dfe.PrepareTransfers.DocumentGeneration/
COPY ./Dfe.PrepareTransfers.Helpers/ ./Dfe.PrepareTransfers.Helpers/
COPY --from=frontend /build/ ./Dfe.PrepareTransfers.Web/

WORKDIR /build/Dfe.PrepareTransfers.Web
RUN --mount=type=secret,id=github_token dotnet nuget add source --username USERNAME --password $(cat /run/secrets/github_token) --store-password-in-clear-text --name github "https://nuget.pkg.github.com/DFE-Digital/index.json"
RUN dotnet restore
RUN dotnet publish -c Release -o /app --no-restore

# Stage 3 - Final
ARG ASPNET_IMAGE_TAG
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final
COPY --from=publish /app /app

WORKDIR /app
COPY ./script/web-docker-entrypoint.sh ./docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh
ENV ASPNETCORE_HTTP_PORTS=80
EXPOSE 80/tcp
