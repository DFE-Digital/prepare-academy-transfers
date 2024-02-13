export class BasePage {

  public continue(): this {

    cy.get('button').contains('Continue').click()

    return this
  }
}
