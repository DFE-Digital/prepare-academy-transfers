describe('Tests to check target date error messages', () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    before(function () {
        cy.login();
    });

    it('Transfer date should be in the future', () => {
        cy.get('[data-test="create-transfer"]').click();
        cy.get('#query').clear();
        cy.get('#query').type('sd');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('[data-test="confirm-outgoing-trust"]').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('#query').clear();
        cy.get('#query').type('ts');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('[data-test="create-project"]').click();
        cy.get('[data-test="transfer-dates"]').click();
        cy.get('[data-test="target-date"]').click();
        cy.fillInDate(Cypress.dayjs().subtract(1,'M'))
        cy.get('.govuk-button').click();
        cy.get('.govuk-error-summary__body > .govuk-list > li > a').should('have.text', 'You must enter a future date').should('be.visible');
        cy.get('#TargetDate\\.Date\\.Day-error').should('have.text', 'Error:You must enter a future date').should('be.visible');
    });

    after(function () {
        cy.clearLocalStorage();
    });

});