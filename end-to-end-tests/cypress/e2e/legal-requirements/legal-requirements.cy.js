/// <reference types ='Cypress'/>

// TO DO: Check Legal Requirement validation on first time use; check Empty tags.

describe('Legal Requirements', { tags: '@dev' }, () => {
    beforeEach(() => {
        cy.selectsFirstProjectOnList()
        cy.get('[data-test="transfer-legal-requirements"]').click()
    })

    it('TC01: Legal Requirement page for project', () => {
        //verify title
        cy.title().should('eq', 'Legal Requirements - Manage an academy transfer')
        // verify heading
        cy.get('h1').contains('Legal requirements')
    })

    it('TC02: Answer to Incoming Trust Agreement and changes current answer from Yes, No, Not Applicable ', () => {
        // Clicks on link
        cy.incomingTrustAgreementLink()
        // verify title
        cy.title().should('eq', 'Incoming trust agreement - Manage an academy transfer')
        // verify heading
        cy.get('h1').contains('Has the incoming trust agreed to take on the academy?')
        // Selects Yes
        cy.get('[id="IncomingTrustAgreement"]').click()
        cy.saveAndContinueButton().click()
        cy.incomingTrustAgreementStatus().should('contain.text', 'Yes')
        // Clicks on link
        cy.incomingTrustAgreementLink()
        // Selects No
        cy.get('[id="No"]').click()
        cy.saveAndContinueButton().click()
        cy.incomingTrustAgreementStatus().should('contain.text', 'No')
        // Clicks on link
        cy.incomingTrustAgreementLink()
        // Selects Not applicable
        cy.get('[id="NotApplicable"]').click()
        cy.saveAndContinueButton().click()
        cy.incomingTrustAgreementStatus().should('contain.text', 'Not applicable')
    })

    it('TC03: Answer to Diocesan consent and changes current answer from Yes, No, Not Applicable', () => {
        // Clicks on change link
        cy.diocesanConsentLink()
        //verify title
        cy.title().should('eq', 'Diocesan consent - Manage an academy transfer')
        // verify heading
        cy.get('h1').contains('Have you spoken with the diocese about the incoming trust?')
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

    it('TC04: Answer to Outgoing Trust consent and changes current answer from Yes, No, Not Applicable', () => {
        // Clicks on change link
        cy.outgoingTrustConsentLink()
        //verify title
        cy.title().should('eq', 'Outgoing trust resolution - Manage an academy transfer')
        // verify heading
        cy.get('h1').contains('Have you received a resolution from the outgoing trust?')
        // Selects Yes
        cy.get('[id="OutgoingTrustConsent"]').click()
        cy.saveAndContinueButton().click()
        cy.outgoingTrustConsentStatus().should('contain.text', 'Yes')
        // Clicks on change link
        cy.outgoingTrustConsentLink()
        // Selects No
        cy.get('[id="No"]').click()
        cy.saveAndContinueButton().click()
        cy.outgoingTrustConsentStatus().should('contain.text', 'No')
        // Clicks on change link
        cy.outgoingTrustConsentLink()
        // Selects Not applicable
        cy.get('[id="NotApplicable"]').click()
        cy.saveAndContinueButton().click()
        cy.outgoingTrustConsentStatus().should('contain.text', 'Not applicable')
    })

    it('TC05: Confirm Legal Requirements page check & marked complete', () => {
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

    it('TC06: Back to task list button link', () => {
        cy.get('[class="govuk-back-link"]')
            .should('be.visible')
            .should('contain.text', 'Back to task list').click()
    })

})