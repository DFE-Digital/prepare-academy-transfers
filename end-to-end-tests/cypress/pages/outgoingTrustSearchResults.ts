class OutgoingTrustSearchResultsPage {

  public slug = 'transfers/trustsearch'

  public selectTrust(trustName): this {

    cy.get('label').contains(trustName).click()

    cy.get('button').contains('Continue').click()

    return this
  }
}

const outgoingTrustSearchResultsPage = new OutgoingTrustSearchResultsPage()

export default outgoingTrustSearchResultsPage
