[![End to end tests](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/end_to_end_tests.yml/badge.svg)](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/end_to_end_tests.yml)

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

## Cypress testing

### Test execution
The Cypress tests will run against the front-end of the application, so the credentials you provide below should be of the user that is set up to run against the UI.

To execute the tests locally and view the output, run the following:

```
cd end-to-end-tests
npm run cypress:open -- --env username='USERNAME',password='PASSWORD',url="BASE_URL_OF_APP"
```

To execute the tests in headless mode, run the following (the output will log to the console):

```
npm run cypress:run -- --env username='USERNAME',password='PASSWORD',url="BASE_URL_OF_APP"
```

To execute the tests and push the results to the cypress dashboard:

```
npm run cypress:run -- --record --key 'KEY' --env username='USERNAME',password='PASSWORD',url="BASE_URL_OF_APP"
```

### Useful tips

#### Maintaining sessions
Each 'it' block usually runs the test with a clear cache. For our purposes, we may need to maintain the user session to test various scenarios. This can be achieved by adding the following code to your tests:

```
afterEach(() => {
		cy.storeSessionData();
	});
```

##### Writing global commands
The cypress.json file in the `support` folder contains functions which can be used globally throughout your tests. Below is an example of a custom login command

```
Cypress.Commands.add("login",()=> {
	cy.visit(Cypress.env('url')+"/login");
	cy.get("#username").type(Cypress.env('username'));
	cy.get("#password").type(Cypress.env('password')+"{enter}");
	cy.saveLocalStorage();
})

```

Which you can access in your tests like so:

```
before(function () {
	cy.login();
});
```

Further details about Cypress can be found here: https://docs.cypress.io/api/table-of-contents
