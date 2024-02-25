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

## Cypress Testing

For detailed information on the Cypress test suite, see the [documentation](end-to-end-tests/README.md)