[![Test, build, and deploy](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/build_test_deploy.yml/badge.svg)](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/build_test_deploy.yml)   [![End to end tests](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/end_to_end_tests.yml/badge.svg)](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/end_to_end_tests.yml)

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
    - `make build-frontend` or 
      - `cd` to `Frontend/wwwroot`
      - run `npm install`
      - run `npm run build`
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

The following user secret is required for the landing page to be able to navigate to the Conversions service:

- `ServiceLink:TransfersUrl` - The URL for the Conversions service.

### Redis

When running Redis locally, you may find it easier to run inside docker, this can be done via the following command:

`docker run -p 6379:6379 --name redis -d redis`

By default the Redis config for the frontend is defined in `appsettings.Development.json` under `VCAP_SERVICES`. This connects to a Redis server 
(without SSL) hosted on `localhost:6379` with an optional password of `password`.

## Authentication and Authorisation
Authentication is provided by Azure AD. The settings are stored in the application configuration and built on startup.
Users can login using their usual DfE account. Allowed roles are `transfers.create`.
Full setup details can be found here: https://docs.microsoft.com/en-us/azure/active-directory/develop/web-app-quickstart?pivots=devlang-aspnet-core

### To add an Azure AD application within Azure
1. Navigate to the appropriate Azure AD within https://portal.azure.com
2. Select App registration and provide a name
3. Add Redirect URI(s), this will end with `/signin-oidc`
4. Select ID tokens
5. Create the required app roles
6. Assign the necessary AD groups or users to the correct app roles within enterprise applications

### To setup receiving a users list of groups within claims
1. Navigate to Token configuration
2. Select "add groups claim"
3. Select "security groups"
4. Group ids will be available in the `HttpContext.User.Claims`

## Model validation
This project uses fluent validators to validate the view model as opposed to MVC Data Annotation. Due to this, using attribute tags such as [Required] will have no effect.

You will need to ensure you have ViewModel to hold the properties you will need to validate and to create a custom validator with an AbstractValidator<CustomViewModel> as it's base class.

Once this is in place then you can use `ModelState.IsValid()` as usual in the controllers.

[FluentValidator Documentation](https://docs.fluentvalidation.net/en/latest/start.html)

## Cypress testing

### Test execution
The Cypress tests will run against the front-end of the application, so the credentials you provide below should be of the user that is set up to run against the UI.

To execute the tests locally and view the output, run the following:

```
cd end-to-end-tests
npm install (first time)
npm run cypress:open -- --env url="BASE_URL_OF_APP",authorizationHeader="secret"
```

To execute the tests in headless mode, run the following (the output will log to the console):

```
npm run cypress:run -- --env url="BASE_URL_OF_APP",authorizationHeader="secret"
```

To execute the tests and push the results to the cypress dashboard:

```
npm run cypress:run -- --record --key 'KEY' url="BASE_URL_OF_APP",authorizationHeader="secret"
```

To execute tests with grep tags on dev:

```
$ npm run cy:run -- --env grepTags=@dev,grepTags=@stage,url="BASE_URL_OF_APP",authorizationHeader="<SECRET HERE>"
```

To execute tests with grep tags on stage:

```
$ npm run cy:run -- --env grepTags=@stage,url="BASE_URL_OF_APP",authorizationHeader="<SECRET HERE>"
```

To only execute all.cy.js file which has all import test files
```
$ npm run cy:run -- --spec "cypress/e2e/all.cy.js", --env grepTags=@stage,url="BASE_URL_OF_APP",authorizationHeader="<SECRET HERE>"
```

We append a wildcard (\*\*) to the URL argument (see `cypress.json`), so it must have a trailing slash (e.g., `url="https://localhost:5001/"`).

### Useful tips

#### Maintaining sessions
Each 'it' block usually runs the test with a clear cache. For our purposes, we may need to maintain the user session to test various scenarios. This can be achieved by adding the following code to your tests:

```
afterEach(() => {
		cy.storeSessionData();
	});
```

##### Writing global commands
The cypress.json file in the `support` folder contains functions which can be used globally throughout your tests. Below is an example of a custom login command.

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
##### AD Sign in
AuthorizationHeader secret passed into the cypress tests needs to match the application configuration
See support/index.js for the cy.intercept command.


Further details about Cypress can be found here: https://docs.cypress.io/api/table-of-contents

To run tests with multiple tags in a list:

```
i.e., greTags=@dev+@stage 
```

To run tests including multiple tags independently targeting individual tags:

```
i.e., grepTags=@dev,grepTags=@stage
```

Further details on using cypress-grep test tagging: https://github.com/cypress-io/cypress-grep 
cypress 10.9.0 Latest changes: https://docs.cypress.io/guides/references/changelog 