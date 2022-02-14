﻿describe('Tests to check first discussed date error messages', () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    before(function () {
        cy.login();
    });

    it('First discussed date should be in the past', () => {
        cy.clickDataTest("create-transfer");
        cy.get('#query').clear();
        cy.get('#query').type('sd');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest("confirm-outgoing-trust");
        cy.selectCheckbox(0);
        cy.get('.govuk-button').click();
        cy.get('#query').clear();
        cy.get('#query').type('ts');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest("create-project");
        cy.clickDataTest("transfer-dates");
        cy.clickDataTest("first-discussed");
        cy.fillInDate(Cypress.dayjs().add(10,'M'))
        cy.get('.govuk-button').click();
        cy.get('.govuk-error-summary__body > .govuk-list > li > a').should('have.text', 'The date the transfer was first discussed with a trust must be in the past').should('be.visible');
        cy.get('span.govuk-error-message').should('have.text', 'Error:The date the transfer was first discussed with a trust must be in the past').should('be.visible');
    });
});
