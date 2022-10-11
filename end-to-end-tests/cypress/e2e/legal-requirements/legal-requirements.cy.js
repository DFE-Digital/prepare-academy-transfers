/// <reference types ='Cypress'/>

// TO DO: Check Legal Requirement validation on first time use; check Empty tags.

describe('Legal Requirements', { tags: '@dev'}, () => {
    beforeEach(() => {
        cy.selectsFirstProjectOnList()
        cy.get('[data-test="transfer-legal-requirements"]').click()
    })

    it('TC01: Answer to Trust Agreement and changes current answer from Yes, No, Not Applicable ', () => {
        // Clicks on link
        cy.trustAgreementLink()
        // Selects Yes
        cy.get('[id="TrustAgreement"]').click()
        cy.saveAndContinueButton().click()
        cy.trustAgreementStatus().should('contain.text', 'Yes')
        // Clicks on link
        cy.trustAgreementLink()
        // Selects No
        cy.get('[id="No"]').click()
        cy.saveAndContinueButton().click()
        cy.trustAgreementStatus().should('contain.text', 'No')
        // Clicks on link
        cy.trustAgreementLink()
        // Selects Not applicable
        cy.get('[id="NotApplicable"]').click()
        cy.saveAndContinueButton().click()
        cy.trustAgreementStatus().should('contain.text', 'Not applicable')
    })

    it('TC02: Answer to Diocesan consent and changes current answer from Yes, No, Not Applicable', () => {
        // Clicks on change link
        cy.diocesanConsentLink()
        // Selects Yes
        cy.get('[id="DiocesanConsent"]').click()
        cy.saveAndContinueButton().click()
        cy.diocesanConsentStatus().should('contain.text', 'Yes')
        // Clicks on change link
        cy.diocesanConsentLink()
        // Selects No
        cy.get('[id="No"]').click()
        cy.saveAndContinueButton().click()
        cy.diocesanConsentStatus().should('contain.text', 'No')
        // Clicks on change link
        cy.diocesanConsentLink()
        // Selects Not applicable
        cy.get('[id="NotApplicable"]').click()
        cy.saveAndContinueButton().click()
        cy.diocesanConsentStatus().should('contain.text', 'Not applicable')
    })

    it('TC03: Answer to Foundation consent and changes current answer from Yes, No, Not Applicable', () => {
        // Clicks on change link
        cy.foundationConsentLink()
        // Selects Yes
        cy.get('[id="FoundationConsent"]').click()
        cy.saveAndContinueButton().click()
        cy.foundationConsentStatus().should('contain.text', 'Yes')
        // Clicks on change link
        cy.foundationConsentLink()
        // Selects No
        cy.get('[id="No"]').click()
        cy.saveAndContinueButton().click()
        cy.foundationConsentStatus().should('contain.text', 'No')
        // Clicks on change link
        cy.foundationConsentLink()
        // Selects Not applicable
        cy.get('[id="NotApplicable"]').click()
        cy.saveAndContinueButton().click()
        cy.foundationConsentStatus().should('contain.text', 'Not applicable')
    })

    it('TC04: Confirm Legal Requirements page check & marked complete', () => {
        cy.selectsFirstProjectOnList().then(() => {
            cy.get('[data-test="legal-requirements"]')
            .invoke('text')
            .then((text) => {
                if (text.includes('COMPLETED')) {
                    return
                }
                else {
                    cy.get('[data-test="transfer-legal-requirements"]').click()
                    cy.get('[data-test="mark-section-complete"]').click()
                    cy.get('[class="govuk-button govuk-!-margin-top-6"]').click()
                    cy.get('[data-test="legal-requirements"]').should('contain.text', 'COMPLETED')
                }
        })
        })
    })

    it('TC05: Back to task list button link', () => {
        cy.get('[class="govuk-back-link"]')
        .should('be.visible')
        .should('contain.text', 'Back to task list').click()
    })

})