Cypress._.each(['ipad-mini'], (viewport) => {
    describe.skip(`107471: Cookie content details on Transfers on ${viewport}`, () => {
      beforeEach(() => {
        cy.viewport(viewport);
        cy.login();
      })
  
      afterEach(() => {
        cy.clearCookies();
      });
  
      it('TC01: should navigate to preference page when user click on collect information link', () => {
        cy.get('[data-test="cookie-banner-link-1"]')
          .should('have.text', 'collect information')
          .click();
        cy.url().then(href => {
          expect(href).includes('/cookie-preferences');
        cy.get('h1').contains('Cookie preferences');  
        });
      });
      
      it('TC02: should navigate to preference page when user click on transfers part of our service link', () => {
        cy.get('[data-test="cookie-banner-transfer-link"]')
          .should('contain', 'transfers part of our service')
          .click();
        cy.url().then(href => {
          expect(href).includes('/home');
        });
      });

      it('TC03: should navigate to preference page when user click on Accept cookies  link', () => {
        cy.get('[data-test="cookie-banner-accept"]')
          .should('contain', 'Accept cookies for the transfers part of this service')
          .click();
        cy.url().then(href => {
          expect(href).includes('/home');
        cy.get('[data-test="create-transfer"]');  
        });
      });

      it('TC04: should navigate to preference page when user click on set your cookie preferences link', () => {
        cy.get('[data-test="cookie-banner-link-2"]')
          .should('have.text', 'set your cookie preferences')
          .click();
        cy.url().then(href => {
          expect(href).includes('/cookie-preferences');
        cy.get('h1').contains('Cookie preferences');  
        });
      });  
    });
});
