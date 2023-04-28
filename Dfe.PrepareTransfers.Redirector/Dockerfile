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



##See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
#
#FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
#WORKDIR /app
#EXPOSE 80
#EXPOSE 443
#
#FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
#WORKDIR /src
#COPY ["Dfe.PrepareTransfers.Redirector/Dfe.PrepareTransfers.Redirector.csproj", "Dfe.PrepareTransfers.Redirector/"]
#RUN dotnet restore "Dfe.PrepareTransfers.Redirector/Dfe.PrepareTransfers.Redirector.csproj"
#COPY . .
#WORKDIR "/src/Dfe.PrepareTransfers.Redirector"
#RUN dotnet build "Dfe.PrepareTransfers.Redirector.csproj" -c Release -o /app/build
#
#FROM build AS publish
#RUN dotnet publish "Dfe.PrepareTransfers.Redirector.csproj" -c Release -o /app/publish /p:UseAppHost=false
#
#FROM base AS final
#WORKDIR /app
#COPY --from=publish /app/publish .
#ENTRYPOINT ["dotnet", "Dfe.PrepareTransfers.Redirector.dll"]