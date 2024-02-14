// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************

// Check accessibility
Cypress.Commands.add('excuteAccessibilityTests', () => {
    const continueOnFail = false
    cy.injectAxe();
    cy.checkA11y(undefined,
    {
        runOnly: {
            type: 'tag',
            values: ['wcag22aa']
        },
        includedImpacts: ["critical", "minor", "moderate", "serious"]
    },
    undefined,
    continueOnFail)
})
