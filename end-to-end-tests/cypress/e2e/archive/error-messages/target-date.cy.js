describe.skip('Tests to check target date error messages', { tags: '@dev'}, () => {
    beforeEach(function () {
        cy.login();
        cy.get('[data-test=cookie-banner-accept]').click();
    });

    it('Transfer date should be in the future', () => {
        cy.clickDataTest("create-transfer");
        cy.get('#SearchQuery').clear();
        //search Trust
        cy.get('#SearchQuery').type('bu');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest("confirm-outgoing-trust");
        cy.selectCheckbox(0);
        cy.get('.govuk-button').click();
        cy.get('#SearchQuery').clear();
        //search outgoing Trust
        cy.get('#SearchQuery').type('ts');
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest("create-project");
        cy.clickDataTest("transfer-dates");
        cy.clickDataTest("target-date");
        cy.fillInDateMonthYear(Cypress.dayjs().subtract(1,'M'))
        cy.get('.govuk-button').click();
        cy.url().should('contain', '/transfer-dates')
    });

    after(function () {
        cy.clearLocalStorage();
    });

});