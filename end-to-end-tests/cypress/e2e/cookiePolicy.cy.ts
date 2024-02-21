describe('Cookie Policy', () => {

  beforeEach(() => {
    cy.visit(Cypress.env('url'))
  })

  it('Should show cookie banner when no preference set', () => {
    cy.get('[data-test="cookie-banner"]').should('be.visible')
  })

  context("When cookie banner clicked", () => {
    beforeEach(() => {
      cy.get('[data-test="cookie-banner-accept"]').click()
    })

    it('Should consent to cookies from cookie header button', () => {
      cy.getCookie('.ManageAnAcademyTransfer.Consent')
      .should('exist')
      .should('have.property', 'value', 'True')
    });

    it('Should hide the cookie banner when consent has been given', () => {
      cy.get("[data-test='cookie-banner']").should('not.exist')
    });
  })

  context("When cookie link in footer clicked", () => {
    beforeEach(() => {
      cy.get("[data-test='cookie-preferences']").click()
    })

    it('Should navigate to cookies page', () => {
      cy.url().then(href => {
        expect(href).includes('cookie-preferences')
      });
    });

    it('Should set cookie preferences', () => {
      cy.get('#cookie-consent-deny').click()
      cy.get("[data-qa='submit']").click()
      cy.getCookie('.ManageAnAcademyTransfer.Consent').should('have.property', 'value', 'False')
    });

    it('Should return show success banner', () => {
      cy.get('#cookie-consent-deny').click()
      cy.get("[data-qa='submit']").click()
      cy.get('[data-test="success-banner-return-link"]').click()
      cy.url().then(href => {
        expect(href).includes('/home')
      });
    });
  })
})