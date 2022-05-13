// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
import "cypress-localstorage-commands";

Cypress.Commands.add('clickBackLink', () => cy.get('.govuk-back-link').click())
Cypress.Commands.add('fillInText', (name, text) => cy.get(`[name="${name}"]`).clear().type(text))
Cypress.Commands.add('fillInTextAtIndex', (index, text) => {
    cy.get('input[type="text"]:visible').then(options => {
        let option = options[index];
        cy.wrap(option).clear().type(text);
    });
});

Cypress.Commands.add('getDataTest', (dataTest) => cy.get(`[data-test='${dataTest}']`))
Cypress.Commands.add('clickDataTest', (dataTest) => cy.getDataTest(dataTest).click())

Cypress.Commands.add('fillInDate', (dayJs) => {
    cy.getDataTest("day").clear().type(dayJs.date())
    cy.getDataTest("month").clear().type(dayJs.month() + 1)
    cy.getDataTest("year").clear().type(dayJs.year())
})

Cypress.Commands.add('selectCheckbox', (index) => {
    cy.get("[type='checkbox']").then(options => {
        let option = options[index]
        option.click()
    });
})

Cypress.Commands.add('selectRadio', (index) => {
    cy.get("[type='radio']").then(options => {
        let option = options[index]
        option.click()
    });
})

Cypress.Commands.add('storeSessionData', () => {
    Cypress.Cookies.preserveOnce('.ManageAnAcademyConversion.Login')
    let str = [];
    cy.getCookies().then((cookie) => {
        cy.log(cookie);
        for (let l = 0; l < cookie.length; l++) {
            if (cookie.length > 0 && l == 0) {
                str[l] = cookie[l].name;
                Cypress.Cookies.preserveOnce(str[l]);
            } else if (cookie.length > 1 && l > 1) {
                str[l] = cookie[l].name;
                Cypress.Cookies.preserveOnce(str[l]);
            }
        }
    });
})

Cypress.Commands.add("login", () => {

    // cy.request({
    //     method: "POST",
    //     url: `https://login.microsoftonline.com/${Cypress.config("tenantId")}/oauth2/token`,
    //     form: true,
    //     body: {
    //         grant_type: "client_credentials",
    //         client_id: Cypress.config("clientId"),
    //         client_secret: Cypress.env("clientSecret"),
    //     },
    // }).then(response => {
    //     const ADALToken = response.body.access_token;
    //     const expiresOn = response.body.expires_on;
    //
    //     localStorage.setItem("adal.token.keys", `${Cypress.config("clientId")}|`);
    //     localStorage.setItem(`adal.access.token.key${Cypress.config("clientId")}`, ADALToken);
    //     localStorage.setItem(`adal.expiration.key${Cypress.config("clientId")}`, expiresOn);
    //     localStorage.setItem("adal.idtoken", ADALToken);
    // });
    //
    // cy.storeSessionData();
    cy.visit(Cypress.env('url'));
})


// ***********************************************
/*  by Asif Ali

    This file is going to be a collection of buttons and links for each page of the transfer journey.
    The idea is using the benefits of the pageObject model used in various JAVA/C# testing frameworks.
    functions will be named according to how they look on the page, and we can identify very easily the error
    should one ocur. Also if code changes, we only have to change in one place instead of every instance.
*/

/*
    Create Journey
*/
// Shared Identifiers
let backLink = '.govuk-back-link'
// PAGE: Manage an academy transfer 
Cypress.Commands.add('HomePage_Button_StartNewProject', () => cy.get('.govuk-button--start'))
Cypress.Commands.add('HomePage_Link_Back', () => cy.get(`${backLink}`))
// PAGE: What is the outgoing trust name?
Cypress.Commands.add('OutGoingSearch_Button_Search', () => cy.get('.govuk-button'))
Cypress.Commands.add('OutGoingSearch_Link_Back', () => cy.get(`${backLink}`))
// PAGE: Select the outgoing trust
// PAGE: Outgoing trust details
// PAGE: Select the transferring academies 
// PAGE: What is the incoming trust name?
// PAGE: Select an incoming trust
// PAGE: Check trust and academy details

/*
    Edit Journey
*/
// PAGE: Transfer Project Main
// PAGE: Feature of Transfer
// PAGE: Set transfer dates
// PAGE: Benefits and risks
// PAGE: Rationale
// PAGE: Trust information and project dates