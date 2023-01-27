# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

# Copy csproj and restore as distinct layers
COPY Dfe.PrepareTransfers.Data.Mock/ ./Dfe.PrepareTransfers.Data.Mock/
COPY Dfe.PrepareTransfers.Data.TRAMS/ ./Dfe.PrepareTransfers.Data.TRAMS/
COPY Dfe.PrepareTransfers.Data/ ./Dfe.PrepareTransfers.Data/
COPY Dfe.PrepareTransfers.DocumentGeneration/ ./Dfe.PrepareTransfers.DocumentGeneration/
COPY Dfe.PrepareTransfers.Helpers/ ./Dfe.PrepareTransfers.Helpers/
COPY Dfe.PrepareTransfers.Web/ ./Dfe.PrepareTransfers.Web/

WORKDIR Dfe.PrepareTransfers.Web
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Dfe.PrepareTransfers.Web.dll"]
