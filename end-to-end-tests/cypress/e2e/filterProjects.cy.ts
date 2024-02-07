import homePage from '../pages/home'

describe.skip('Filter projects', () => {

  beforeEach(() => {
    cy.visit(Cypress.env('url'))
  })

  it('is a wip', () => {

    homePage.createNewTransfer()
  })
})