let outgoingTrustId = '10059580';
let incomingTrustId = '10060470';

describe('Select multiple academies', { tags: '@dev'},  () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    beforeEach(function () {
        cy.login();
        cy.get('[data-test=cookie-banner-accept]').click();
    });
    
    it('Check your answers page should have both academies listed', function() {
        cy.clickDataTest("create-transfer");
        cy.get('#SearchQuery').clear();
        cy.get('#SearchQuery').type(outgoingTrustId);
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.clickDataTest('confirm-outgoing-trust');
        cy.selectCheckbox(0);
        cy.selectCheckbox(1);
        cy.get('.govuk-button').click();
        cy.get('#SearchQuery').clear();
        cy.get('#SearchQuery').type(incomingTrustId);
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('.govuk-grid-column-full > :nth-child(3)').should('have.text', 'Longsight Community Primary');
        cy.get('.govuk-grid-column-full > :nth-child(5)').should('have.text', 'Unity Community Primary');
        cy.get(':nth-child(8) > :nth-child(1) > .govuk-summary-list__value').should('have.text', 'Burntwood Trust');
        cy.get(':nth-child(8) > :nth-child(2) > .govuk-summary-list__value').should('have.text', incomingTrustId);
        cy.get(':nth-child(2) > :nth-child(1) > .govuk-summary-list__value').should('have.text', 'Big Life Schools');
        cy.get(':nth-child(2) > :nth-child(2) > .govuk-summary-list__value').should('have.text', outgoingTrustId);
    });
});