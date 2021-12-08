let trustId = '10060470';

describe('Tests to check search incoming trust error message', () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    before(function () {
        cy.login();
    });

    it('Should not show the outgoing trust in search results', () => {
        cy.get('[data-test="create-transfer"]').click();
        cy.get('#query').clear();
        cy.get('#query').type(trustId);
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('[data-test="confirm-outgoing-trust"]').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('#query').clear();
        cy.get('#query').type(trustId);
        cy.get('.govuk-button').click();
        cy.get('.govuk-error-summary__body > .govuk-list > li > a').should('have.text', 'We could not find any trusts matching your search criteria').should('be.visible');
        cy.get('#query-error').should('have.text', '\n                            Error: We could not find any trusts matching your search criteria\n                        ').should('be.visible');
    });

    after(function () {
        cy.clearLocalStorage();
    });

});