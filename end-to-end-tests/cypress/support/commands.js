// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
//
//
// -- This is a parent command --
// Cypress.Commands.add('login', (email, password) => { ... })
//
//
// -- This is a child command --
// Cypress.Commands.add('drag', { prevSubject: 'element'}, (subject, options) => { ... })
//
//
// -- This is a dual command --
// Cypress.Commands.add('dismiss', { prevSubject: 'optional'}, (subject, options) => { ... })
//
//
// -- This will overwrite an existing command --
// Cypress.Commands.overwrite('visit', (originalFn, url, options) => { ... })
import "cypress-localstorage-commands";

Cypress.Commands.add('clickBackLink', () => cy.get('.govuk-back-link').click())
Cypress.Commands.add('fillInText', (name, text) => cy.get(`[name="${name}"]`).clear().type(text))
Cypress.Commands.add('fillInTextAtIndex', (index, text) =>{
 cy.get('input[type="text"]:visible').then(options => {
     let option = options[index];
     cy.wrap(option).clear().type(text);
 });
});
Cypress.Commands.add('fillInDate', (day, month, year) => {
    cy.get('[data-test="day"]').clear().type(day)
    cy.get('[data-test="month"]').clear().type(month)
    cy.get('[data-test="year"]').clear().type(year)
})
Cypress.Commands.add('selectCheckbox', (index) => {
    cy.get("[type='checkbox']").then(options => {
        let option = options[index];
        option.click();
    });
})

Cypress.Commands.add('selectRadio', (index) => {
    cy.get("[type='radio']").then(options => {
        let option = options[index];
        option.click();
    });
})

Cypress.Commands.add('storeSessionData',()=>{
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
