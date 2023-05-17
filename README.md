[![build-and-push-image.yml](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/build-and-push-image.yml/badge.svg)](https://github.com/DFE-Digital/academy-transfers-api/actions/workflows/build-and-push-image.yml)

# academy-transfers-api

The internal service for managing Academy Transfers.

## Tech notes

### Quickstart

To get the Dfe.PrepareTransfers.Web project running you will require:

- .NET Core 3.1
- Node v18.x

1. **Install dependencies**
- .NET
    - `dotnet restore`
- Node
    - `make build-frontend` or
      - `cd` to `Dfe.PrepareTransfers.Web/wwwroot`
      - run `npm install`
      - run `npm run build`
1. **Set user secrets**
   - `dotnet user-secrets set "Key" "Value" --project Dfe.PrepareTransfers.Web`
2. **Run the application**
- `dotnet run --project Dfe.PrepareTransfers.Web`

### User-secrets

**Dfe.PrepareTransfers.Web**

The frontend repo requires the following user secrets to be able to connect to TRAMS:

- `TRAMS_API_KEY` - The API key for the TRAMS API
- `TRAMS_API_BASE` - The base URL for the TRAMS API (with trailing slash)

The following user secret is required for the landing page to be able to navigate to the Conversions service:

- `ServiceLink:TransfersUrl` - The URL for the Conversions service.

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
This project uses fluent validators to validate the view model as opposed to MVC Dfe.PrepareTransfers.Data Annotation. Due to this, using attribute tags such as [Required] will have no effect.

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
```
##### Cypress Linting
 Cypress ESLint Plugin
 ESLint is a tool for identifying and reporting on patterns found in ECMAScript/JavaScript code, with the goal of making code more consistent and avoiding bugs.

 Note: If you installed ESLint globally then you must also install eslint-plugin-cypress globally.

 -Installation using npm
  `npm install eslint-plugin-cypress --save-dev`

 -Installation using yarn
  `yarn add eslint-plugin-cypress --dev`

 -Usage: Add an .eslintrc.json file to your cypress directory with the following:
   {
      "plugins": [
      "cypress"
     ]
    }

-Add rules, example:
  {
    "rules": {
      "cypress/no-assigning-return-values": "error",
      "cypress/no-unnecessary-waiting": "error",
      "cypress/assertion-before-screenshot": "warn",
      "cypress/no-force": "warn",
      "cypress/no-async-tests": "error",
      "cypress/no-pause": "error"
    }
  }
 -Use the recommended configuration and you can forego configuring plugins, rules, and env individually.
  {
    "extends": [
      "plugin:cypress/recommended"
    ]
  }
```
##### Git hook for esLint
Git hooks provide a way to fire off custom scripts on different events such as during commit, push or rebase, etc

### Husky
Husky can prevent you from bad git commit, git push and more.

-Install
`npm install husky -D`

-Usage
Edit package.json > prepare script and run it once:

`npm pkg set scripts.prepare="husky install"`
`npm run prepare`

-Add a hook:

`npx husky add .husky/pre-commit "npm run lint"`
`git add .husky/pre-commit`

-Make a commit:

`git commit -m "Keep calm and commit"`
esLint will run befor the commit
```