class HomePage {

  public slug = 'home'

  public open(): this {

    cy.visit(Cypress.env('url'))
    return this
  }

  public startCreateNewTransfer(): this {

    cy.get('[data-test=create-transfer]').click()

    return this
  }

  public projectsCountShouldBeVisible(): this {

    cy.get('[data-cy="select-projectlist-filter-count"]').should('be.visible')
    cy.get('[data-cy="select-projectlist-filter-count"]').should('contain.text', 'projects found')

    return this
  }

  public getProjectsCount(): Number {

    let projectsCount = 0

    cy.get('[data-cy="select-projectlist-filter-count"]')
      .invoke('text')
      .then((txt) => {
        projectsCount = Number(txt.split(' ')[0])
      })

    return projectsCount
  }

  public toggleFilterProjects(isOpen): this {
    
    cy.get('[data-cy="select-projectlist-filter-expand"]').click()
    if(isOpen)
      cy.get('details').should('have.attr', 'open')
    else
      cy.get('details').should('not.have.attr', 'open')

    return this
  }

  public filterProjects(projectTitle): this {

    cy.get('[id="Title"]').type(projectTitle)

    cy.get('[data-cy="select-projectlist-filter-apply"]').click()

    cy.get('[data-module="govuk-notification-banner"]').should('be.visible')
    cy.get('[data-module="govuk-notification-banner"]').should('contain.text', 'Success')
    cy.get('[data-module="govuk-notification-banner"]').should('contain.text', 'Projects filtered')

    cy.get('[data-cy="select-projectlist-filter-count"]').should('be.visible')
    cy.get('[data-cy="select-projectlist-filter-count"]').should('contain.text', 'projects found')

    cy.get('tbody > tr').should('have.length.at.least', 1)

    return this
  }

  public clearFilters(): this {

    cy.get('[data-cy="clear-filter"]').click()

    return this
  }

  public selectFirstProject(): this {

    cy.get('[data-id*="project-link"]').first().click()
    return this
  }
}

const homePage = new HomePage()

export default homePage
