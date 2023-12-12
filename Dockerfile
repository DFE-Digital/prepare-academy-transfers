ARG ASPNET_IMAGE_TAG=6.0-bullseye-slim
ARG NODEJS_IMAGE_TAG=18.12-bullseye

# Stage 1 - Build project
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS publish
WORKDIR /build

COPY ./Dfe.PrepareTransfers.Data/ ./Dfe.PrepareTransfers.Data/
COPY ./Dfe.PrepareTransfers.Data.TRAMS/ ./Dfe.PrepareTransfers.Data.TRAMS/
COPY ./Dfe.PrepareTransfers.DocumentGeneration/ ./Dfe.PrepareTransfers.DocumentGeneration/
COPY ./Dfe.PrepareTransfers.Helpers/ ./Dfe.PrepareTransfers.Helpers/
COPY ./Dfe.PrepareTransfers.Web/ ./Dfe.PrepareTransfers.Web/

WORKDIR /build/Dfe.PrepareTransfers
RUN --mount=type=secret,id=github_token dotnet nuget add source --username USERNAME --password $(cat /run/secrets/github_token) --store-password-in-clear-text --name github "https://nuget.pkg.github.com/DFE-Digital/index.json"
RUN dotnet restore Dfe.PrepareTransfers.sln
RUN dotnet build -c Release Dfe.PrepareTransfers.sln --no-restore
RUN dotnet publish Dfe.PrepareTransfers.Web -c Release -o /app --no-restore

# Stage 2 - Build assets
FROM node:${NODEJS_IMAGE_TAG} as build
COPY --from=publish /app /app
WORKDIR /app/wwwroot
RUN npm install
RUN npm run build

# Stage 3 - Final
ARG ASPNET_IMAGE_TAG
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final
COPY --from=publish /app /app

WORKDIR /app
COPY ./script/web-docker-entrypoint.sh ./docker-entrypoint.sh
RUN chmod +x ./docker-entrypoint.sh
EXPOSE 80/tcp
