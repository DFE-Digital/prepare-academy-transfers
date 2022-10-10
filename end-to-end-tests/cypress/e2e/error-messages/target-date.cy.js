describe('Tests to check target date error messages', { tags: '@dev'}, () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    beforeEach(function () {
        cy.login();
    });

    it('Transfer date should be in the future', () => {
        cy.clickDataTest("create-transfer");
        cy.get('#SearchQuery').clear();
        cy.get('#SearchQuery').type('sd');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest("confirm-outgoing-trust");
        cy.selectCheckbox(0);
        cy.get('.govuk-button').click();
        cy.get('#SearchQuery').clear();
        cy.get('#SearchQuery').type('ts');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest("create-project");
        cy.clickDataTest("transfer-dates");
        cy.clickDataTest("target-date");
        cy.fillInDateMonthYear(Cypress.dayjs().subtract(1,'M'))
        cy.get('.govuk-button').click();
        cy.get('.govuk-error-summary__body > .govuk-list > li > a').should('have.text', 'You must enter a future date').should('be.visible');
        //cy.get('#TargetDateViewModel\\.TargetDate\\.Date\\.Day-error').should('have.text', 'Error:You must enter a future date').should('be.visible');
    });

    after(function () {
        cy.clearLocalStorage();
    });

});