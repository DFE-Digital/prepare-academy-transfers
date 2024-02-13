# Cypress Tests

This page outlines the approach to the Cypress test suite for the project.

## Setup

Install dependencies by running `npm install` inside the `end-to-end-tests` directory.

### Secrets management

The Cypress tests rely on some secrets being set. To do so, create a `cypress.env.json` file in the `end-to-end-tests` directory with the following information:

```
{
  "url": <your-url-here>,
  "authorizationHeader": <your-header-here>
}
```

Replace the values as required based on the below:

| Key | Description | Example |
|--|--|--|
| `url` | The base url for the application | `https://localhost:5005`<sup>1</sup> |
| `authorizationHeader` | The header used to bypass auth for test environments | `abc-123` |

<sup>1</sup> We append a wildcard (\*\*) to the `url` argument (see the [support file](cypress/support/e2e.ts)), so it must have a trailing slash (e.g., `url="https://localhost:5001/"`).

## Test execution
The Cypress tests will run against the front-end of the application.

To execute the tests locally and view the output, run the following:

`npx cypress open`

To execute the tests in headless mode, run the following (the output will log to the console):

`npx cypress run`

## Cypress Linting
We make use of [ESLint](https://eslint.org/) for code formatting and styling. This is to help make code more consistent and avoid bugs.

Linting is performed on Pull Requests to ensure standards are upheld, but can also be ran locally using the following command:

`npm run lint`

By default, all recommended checks are set to `Error`.

## Security testing with ZAP

The Cypress tests can also be run, proxied via OWASP ZAP for passive security scanning of the application.

These can be run using the configured docker-compose.yml, which will spin up containers for the ZAP daemon and the Cypress tests, including all networking required. You will need to update any config in the file before running

Create a `.env` file for docker, this file needs to include

* all of your required cypress configuration
* HTTP_PROXY e.g. http://zap:8080
* ZAP_API_KEY, can be any random guid

Example env:
```
URL=<Enter URL>
API_KEY=<Enter API key>
HTTP_PROXY=http://zap:8080
ZAP_API_KEY=<Enter random guid>
```
_Note: You might have trouble running this locally because of docker thinking localhost is the container and not your machine_

To run docker compose use:

`docker-compose -f docker-compose.yml --exit-code-from cypress`

_Note: `--exit-code-from cypress` tells the container to quit when cypress finishes_

You can also exclude URLs from being intercepted by using the NO_PROXY setting

e.g. `NO_PROXY=*.google.com,yahoo.co.uk`

Alternatively, you can run the Cypress tests against an existing ZAP proxy by setting the environment configuration
```
HTTP_PROXY="<zap-daemon-url>"
NO_PROXY="<list-of-urls-to-ignore>"
```
and setting the runtime variables

`zapReport=true,zapApiKey=<zap-api-key>,zapUrl="<zap-daemon-url>"`