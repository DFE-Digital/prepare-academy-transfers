# Stage 1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /build

# Copy csproj and restore as distinct layers
COPY Data.Mock/ ./Data.Mock/
COPY Data.TRAMS/ ./Data.TRAMS/
COPY Data/ ./Data/
COPY DocumentGeneration/ ./DocumentGeneration/
COPY Helpers/ ./Helpers/
COPY Frontend/ ./Frontend/

WORKDIR Frontend
RUN dotnet restore
RUN dotnet publish -c Release -o /app

# Stage 2
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "Frontend.dll"]
