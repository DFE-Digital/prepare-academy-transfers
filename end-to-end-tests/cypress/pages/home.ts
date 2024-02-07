class HomePage {

  public slug = 'home'

  public startCreateNewTransfer(): this {

    cy.get('[data-test=create-transfer]').click()

    return this
  }

  public filterProjects(): this {

    cy.get('[data-cy="select-projectlist-filter-expand"]').click()

    return this
  }
}

const homePage = new HomePage()

export default homePage
