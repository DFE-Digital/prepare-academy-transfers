class NewTransferPage {

  public slug = 'transfers/newtransfersinformation'

  public clickCreateNewTransfer(): this {

    cy.get('.govuk-button').contains('Create a new transfer').click()

    return this
  }
}

const newTransferPage = new NewTransferPage()

export default newTransferPage
