// Scans each page for accessibility violations
// Note: there are some pages that cannot be scanned in this way as they require
// session data setting so must be done manually -
// transfers/outgoingtrustacademies and transfers/checkyouranswers

describe('Check accessibility of the different pages', () => {
    it('Validate accessibility', () => {
        cy.fixture('transfersLinks.json').then((transfersLinks) => {
            transfersLinks.forEach((link) => {
                cy.visit(`${Cypress.env('url')}${link}`)
                cy.excuteAccessibilityTests()
            })
        })
    })
})
