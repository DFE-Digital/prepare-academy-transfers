class OutgoingTrustSearchPage {

  public slug = 'transfers/trustname'

  public searchTrustsByName(trustName): this {

    cy.get('#SearchQuery').type(trustName)

    cy.get('button').contains('Search').click()

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

const outgoingTrustSearchPage = new OutgoingTrustSearchPage()

export default outgoingTrustSearchPage
