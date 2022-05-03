let trustId = '10060470';

describe('Tests to check search incoming trust error message', () => {
    afterEach(() => {
        cy.storeSessionData();
    });


    it('Logs into AD', () => {
        cy.login();
    });
    
    it('Should not show the outgoing trust in search results', () => {
        cy.visit("https://academy-transfers-dev.london.cloudapps.digital/");
        cy.get('[data-test="create-transfer"]').click();
        cy.get('#SearchQuery').clear();
        cy.get('#SearchQuery').type(trustId);
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('[data-test="confirm-outgoing-trust"]').click();
        cy.selectCheckbox(0);
        cy.get('.govuk-button').click();
        cy.get('#SearchQuery').clear();
        cy.get('#SearchQuery').type(trustId);
        cy.get('.govuk-button').click();
        cy.get('.govuk-error-summary__body > .govuk-list > li > a').should('have.text', 'We could not find any trusts matching your search criteria').should('be.visible');
        cy.get('#SearchQuery-error').should('have.text', 'Error:We could not find any trusts matching your search criteria').should('be.visible');
    });

    after(function () {
        cy.clearLocalStorage();
    });
});