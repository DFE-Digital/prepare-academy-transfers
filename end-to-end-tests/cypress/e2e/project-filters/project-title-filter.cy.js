/// <reference types ='Cypress'/>

Cypress._.each(['ipad-mini'], (viewport) => {
    describe(`110767: Filter projects and verify the count ${viewport}`, () => {
      beforeEach(() => {
        cy.viewport(viewport);
        cy.login();
        cy.navigateToFilterProjects();
      })
  
      afterEach(() => {
        cy.clearCookies();
      });
  
      it('TC01: should display the result when match found for the project title', () => {
        cy.get('[id="TitleFilter"]').type('The Three Saints Academy Trust')
        cy.get('[data-cy=select-projectlist-filter-apply]').click();
        cy.get('[data-cy="select-projectlist-filter-count"]').should('not.have.text', '0 projects found');
        cy.get('.govuk-table__body > :nth-child(1)').should('contain', 'The Three Saints Academy Trust')
      });

      it('TC02: results should display 0 count when no match found for the projects title', () => {
        cy.get('[id="TitleFilter"]').type('random')
        cy.get('[data-cy=select-projectlist-filter-apply]').click();
        cy.get('[data-cy="select-projectlist-filter-count"]').should('contain', '0 projects found');
      });   
  
      it('TC02: extra space should get trim when user may accidentally enter a space in the beginning or end', () => {
        cy.get('[id="TitleFilter"]').type(' Excell3 Independent Schools Ltd ')
        cy.get('[data-cy=select-projectlist-filter-apply]').click();
        cy.get('[data-cy="select-projectlist-filter-count"]').should('not.have.text', '0 projects found');
        cy.get('[id="pagination-label"]').should('be.visible');

      });
    });  
  });
  