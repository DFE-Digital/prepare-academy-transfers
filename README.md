# academy-transfers-api

The internal service for managing Academy Transfers.

## Tech notes

### Quickstart

To get the Frontend project running you will require:

- .NET Core 3.1
- Redis (running locally or in Docker, see below)
- Node v12.x

1. **Install dependencies**
- .NET
    - `dotnet restore`
- Node
    - `make build-frontend`
2. **Ensure Redis is running**
3. **Set user secrets**
   - `dotnet user-secrets set "Key" "Value" --project Frontend`
4. **Run the application**
- `dotnet run --project Frontend`

### User-secrets

**Frontend**

The frontend repo requires the following user secrets to be able to connect to TRAMS:

- `TRAMS_API_KEY` - The API key for the TRAMS API
- `TRAMS_API_BASE` - The base URL for the TRAMS API (with trailing slash)

### Redis

When running Redis locally, you may find it easier to run inside docker, this can be done via the following command:

`docker run -p 6379:6379 --name redis -d redis`

By default the Redis config for the frontend is defined in `appsettings.Development.json` under `VCAP_SERVICES`. This connects to a Redis server 
(without SSL) hosted on `localhost:6379` with an optional password of `password`.
