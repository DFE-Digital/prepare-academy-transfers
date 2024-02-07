export class BasePage {

  public continueTransfer(): this {

    cy.get('button').contains('Continue').click()

    return this
  }
}