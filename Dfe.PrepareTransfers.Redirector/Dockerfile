FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["Dfe.PrepareTransfers.Redirector/Dfe.PrepareTransfers.Redirector.csproj", "Dfe.PrepareTransfers.Redirector/"]
RUN dotnet restore "Dfe.PrepareTransfers.Redirector/Dfe.PrepareTransfers.Redirector.csproj"
COPY . .

# Stage 2 - Build assets
FROM node:18.12-bullseye as frontend
COPY --from=build /src /src
WORKDIR /src/Dfe.PrepareTransfers/wwwroot
RUN npm install
RUN npm run build
RUN mkdir -p /src/Dfe.PrepareTransfers.Redirector/wwwroot/css/
RUN mv dist/site.css /src/Dfe.PrepareTransfers.Redirector/wwwroot/css/site.css

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS sdk
COPY --from=frontend /src /src
WORKDIR /src/Dfe.PrepareTransfers.Redirector
RUN dotnet build "Dfe.PrepareTransfers.Redirector.csproj" -c Release
RUN dotnet publish "Dfe.PrepareTransfers.Redirector.csproj" -c Release -o /app /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS final
COPY --from=sdk /app /app
WORKDIR /app
ENTRYPOINT ["dotnet", "Dfe.PrepareTransfers.Redirector.dll"]