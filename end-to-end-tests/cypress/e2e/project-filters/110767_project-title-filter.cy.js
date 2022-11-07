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
        cy.get('[data-cy="select-projectlist-filter-title"]').type('The Three Saints Academy Trust')
        cy.get('[data-cy=select-projectlist-filter-apply]').click();
        cy.get('[data-cy="select-projectlist-filter-count"]').should('not.have.text', '0 projects found');
        cy.get('[class="govuk-table__row"]').first().should('contain', 'The Three Saints Academy Trust')
      });

      it('TC02: results should display 0 count when no match found for the projects title', () => {
        cy.get('[id="TitleFilter"]').type('test')
        cy.get('[data-cy=select-projectlist-filter-apply]').click();
        cy.get('[data-cy="select-projectlist-filter-count"]').should('contain', '0 projects found');
      });   
  
      it('TC02: extra space should get trim when user may accidentally enter a spcae in the beginning or end', () => {
        cy.get('[id="TitleFilter"]').type(' The Three Saints Academy Trust ')
        cy.get('[data-cy=select-projectlist-filter-apply]').click();
        cy.get('[data-cy="select-projectlist-filter-count"]').should('not.have.text', '0 projects found');
        cy.get('[data-cy="select-projectlist-filter-count"]')
          .invoke('text')
          .should('match', /^[0-9]\D*/);
      });
    });  
  });
  