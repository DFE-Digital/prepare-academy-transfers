/// <reference types ='Cypress'/>

const url = Cypress.env('url') + 'project-type'

describe('Landing Page', { tags: '@dev'}, () => {
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
        cy.get('[data-cy="select-projecttype-input-transfer"]').click()
        cy.get('[data-cy="select-common-submitbutton"]').click()
        cy.get('[data-cy="select-heading"]').should('contain.text', 'Manage an academy transfer')
        cy.get('[data-cy="select-backlink"]').click()
        cy.get('[data-cy="select-heading"]').should('contain.text', 'What do you want to do?')
    })

    it('TC02: Checks error message on unselected project', () => {
        cy.continueBtn().click()
        cy.get('[data-qa="error_text"]').should('contain.text', 'Select a project type')
    })
})