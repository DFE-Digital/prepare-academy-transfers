/// <reference types ='Cypress'/>
import transfersLinks from '../../fixtures/transfersLinks.json'


const wcagStandards = ["wcag22aa"];
const impactLevel = ["critical", "minor", "moderate", "serious"];
const continueOnFail = false;

Cypress._.each(['ipad-mini'], (viewport) => {
    describe('Check accessibility of the different pages', function () {
        transfersLinks.forEach((link) => {

            it('Validate accessibility on different pages on Dev env.', function () {
                let url = Cypress.env('url')
                if (url.toString().includes('dev')) {
                    cy.visit(url)
                    cy.visit(url + link)
                    cy.excuteAccessibilityTests(wcagStandards, continueOnFail, impactLevel)
                }
                else {
                    cy.log('Staging env.') //Accessibility check only at Dev. env. atm
                    Cypress.runner.stop()
                }
            })
        })
    })
})
