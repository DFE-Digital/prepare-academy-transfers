ARG ASPNET_IMAGE_TAG=6.0-bullseye-slim
ARG NODEJS_IMAGE_TAG=18.12-bullseye

# Stage 1 - Node Assets
FROM node:${NODEJS_IMAGE_TAG} as frontendBuild
COPY . /build/
WORKDIR /build/wwwroot
RUN npm install
Run npm run build

# Stage 2 - DotNet
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS dotnetBuild
WORKDIR /build
COPY . .
RUN  rm -rf ./Dfe.PrepareTransfers.Redirector/wwwroot/src
RUN  rm -rf ./Dfe.PrepareTransfers.Redirector/wwwroot/dist
COPY --from=frontendBuild /build/ ./Dfe.PrepareTransfers.Redirector

# Stage 3 - Publish
WORKDIR /build/Dfe.PrepareTransfers.Redirector
RUN dotnet restore
RUN dotnet build --configuration Release
RUN dotnet publish --no-build --configuration Release --output /app

# Stage 4 - Finalise
ARG ASPNET_IMAGE_TAG
FROM "mcr.microsoft.com/dotnet/aspnet:${ASPNET_IMAGE_TAG}" AS final
COPY --from=dotnetBuild /app /app
WORKDIR /app
EXPOSE 80/tcp
ENTRYPOINT ["dotnet", "Dfe.PrepareTransfers.Redirector.dll"]