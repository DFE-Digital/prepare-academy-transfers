/// <reference types ='Cypress'/>

Cypress._.each(['ipad-mini'], (viewport) => {
	describe(`110944 Cookie consent details for Transfers on ${viewport}`, () => {
		beforeEach(() => {
			cy.login()
			cy.viewport(viewport)
			cy.selectTrustListing(2)
			cy.url().then(url => {
				//Changes the current URL
				let modifiedUrl = url + '/transfer-dates'
				cy.visit(modifiedUrl)
			});
		})

		it('TC01: should show top cookie banner when no consent option set', () => {
			cy.get('[data-test="cookie-banner"]').should('be.visible')
		});

		context("when cookie banner clicked", () => {
			beforeEach(() => {
				cy.get('[data-test="cookie-banner-accept"]').click()
			})

			it('TC02: should consent to cookies from cookie header button', () => {
				cy.getCookie('.ManageAnAcademyTransfer.Consent')
				.should('exist')
				.should('have.property', 'value', 'True')
			});

			it('TC03: should hide the cookie banner when consent has been given', () => {
				cy.get("[data-test='cookie-banner']").should('not.exist')
			});
		})

		context("when cookie link in footer clicked", () => {
			beforeEach(() => {
				cy.get("[data-test='cookie-preferences']").click()
			})

			it('TC04: should navigate to cookies page', () => {
				cy.url().then(href => {
					expect(href).includes('cookie-preferences')
				});
			});

			it('TC05: should set cookie preferences', () => {
				cy.get('#cookie-consent-deny').click()
				cy.get("[data-qa='submit']").click()
				cy.getCookie('.ManageAnAcademyTransfer.Consent').should('have.property', 'value', 'False')
			});

			it('TC06: should return show success banner', () => {
				cy.get('#cookie-consent-deny').click()
				cy.get("[data-qa='submit']").click()
				cy.get('[data-test="success-banner-return-link"]').click()
				cy.url().then(href => {
					expect(href).includes('/transfer-dates')
				});
			});
		})
	});
});
