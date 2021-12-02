let academyId = '10061054';

describe('Tests to ensure correct transfer functionality', () => {
    afterEach(() => {
		cy.storeSessionData();
	});

	before(function () {
		cy.login();
	});

    it('Button should be visible', () => {
        cy.get('.govuk-button.govuk-button--start').as('startButton')
        cy.get('@startButton').should('be.visible')
        cy.get('@startButton').click()
    });

    it('Should allow user to search for Academy', () => {
            cy.get('.govuk-input').type(academyId+'{enter}')
        cy.get('.govuk-radios__item input').click()
        cy.get('.govuk-button').click()
        cy.get('.govuk-button').click()
    });

    it.skip('Should prevent a user from transferring to the same trust', () => {
        cy.get('.govuk-radios__item input').click()
        cy.get('.govuk-button').click()
        cy.get('.govuk-input').type(academyId+'{enter}')
        // Once 81405 is fixed, we should add any error messaging or assertions below
        // to ensure a user cannot transfer to the same trust
        cy.get('.govuk-radios__item input').click()
        cy.get('.govuk-button').click()


    });


    

    after(function () {
        cy.clearLocalStorage();
    });
    
});