describe('Academy Transfer Tests', () => {
    it('Creates a transfer project', () => {
        cy.visit('/')

        // if asks for username and password enter
        cy.url().should('include', '/home/login')

        cy.get('.govuk-input#username')
            .type('username')
            .should('have.value', 'username')

        cy.get('.govuk-input#password')
            .type('password')
            .should('have.value', 'password')
        
        cy.get('button.govuk-button--primary').click()
        
        cy.contains('Start a new transfer project').click()

        cy.url().should('include', '/transfers/trustname')

        cy.get('input.govuk-input#query')
            .type('burntwood trust')
            .should('have.value', 'burntwood trust')
        
        cy.get('button[type=submit]').click()
        
        cy.url().should('include', 'transfers/trustsearch?query=burntwood+trust&change=False')
        
        cy.get('input#10060470').click()

        // need to change this to submit button on view
        cy.get('button').click()
        
    })
})