describe('Tests to check advisory board date error messages', { tags: '@dev'}, () => {
    beforeEach(function () {
        cy.login();
        cy.get('[data-test=cookie-banner-accept]').click();
    });

    it('Advisory board date should be in the future', () => {
        cy.login();
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
        cy.clickDataTest("ab-date");
        cy.fillInDate(Cypress.dayjs().subtract(1,'M'))
        cy.get('.govuk-button').click();
        cy.get('.govuk-error-summary__body > .govuk-list > li > a').should('have.text', 'You must enter a future date').should('be.visible');
        cy.get('#AdvisoryBoardViewModel\\.AdvisoryBoardDate\\.Date\\.Day-error').should('have.text', 'Error:You must enter a future date').should('be.visible');
    });

    after(function () {
        cy.clearLocalStorage();
    });

});