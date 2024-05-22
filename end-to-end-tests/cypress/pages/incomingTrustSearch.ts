class IncomingTrustSearchPage {

  public slug = 'transfers/searchincomingtrust'

  public searchTrustsByName(trustName): this {

    cy.get('[data-cy="ProposedTrustNameID"]').type(trustName)

    cy.get('button').contains('Continue').click()

    return this
  }

  public searchTrustsByUkprn(ukprn): this {

    cy.get('#SearchQuery').type(ukprn)

    cy.get('button').contains('Search').click()

    return this
  }

  public searchTrustsByCompaniesHouseNo(companiesHouseNo): this {

    cy.get('#SearchQuery').type(companiesHouseNo)

    cy.get('button').contains('Search').click()

    return this
  }
}

const incomingTrustSearchPage = new IncomingTrustSearchPage()

export default incomingTrustSearchPage
