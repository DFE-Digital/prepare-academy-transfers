let outgoingTrustId = '10059580';
let incomingTrustId = '10060470';

describe('Select multiple academies', () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    before(function () {
        cy.login();
    });
    
    it('Check your answers page should have both academies listed', function() {
        cy.clickDataTest("create-transfer");
        cy.get('#query').clear();
        cy.get('#query').type(outgoingTrustId);
        cy.get('.govuk-button').click();
        cy.selectRadio(0);
        cy.get('.govuk-button').click();
        cy.get('[data-test="confirm-outgoing-trust"]').click();
        cy.selectCheckbox(0);
        cy.selectCheckbox(1);
        cy.get('.govuk-button').click();
        cy.get('#query').clear();
        cy.get('#query').type(incomingTrustId);
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