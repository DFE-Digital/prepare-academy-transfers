// <reference types ='Cypress'/>

const url = Cypress.env('url') + 'project-type'

describe('Landing Page',() => {
    beforeEach(() => {
        cy.visit(url)
    })

    afterEach(() => {
        cy.storeSessionData()
    })

    after(function () {
        cy.clearLocalStorage()
    })

    it('TC01: Navigates to Transfer site from landing page, and clicks back to landing page', () => {
        cy.get('[id="transfer-radio"]').click()
        cy.get('[id="submit-btn"]').click()
        cy.get('[class="govuk-heading-xl"]').should('contain.text', 'Manage an academy transfer')
        cy.get('[class="govuk-back-link"]').click()
        cy.get('[class="govuk-heading-l"]').should('contain.text', 'What do you want to do?')
    })
})