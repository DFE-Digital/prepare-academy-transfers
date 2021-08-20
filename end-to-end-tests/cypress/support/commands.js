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

Cypress.Commands.add('clickBackLink', () => cy.get('.govuk-back-link').click())
Cypress.Commands.add('fillInText', (name, text) => cy.get(`[name="${name}"]`).clear().type(text))
Cypress.Commands.add('fillInTextAtIndex', (index, text) =>{
 cy.get('input[type="text"]:visible').then(options => {
     let option = options[index];
     cy.wrap(option).clear().type(text);
 });
});
Cypress.Commands.add('fillInDate', (day, month, year) => {
    cy.get('[name="day"]').clear().type(day)
    cy.get('[name="month"]').clear().type(month)
    cy.get('[name="year"]').clear().type(year)
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
