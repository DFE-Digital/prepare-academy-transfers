import { BasePage } from './basePage'

class OutgoingTrustDetailsPage extends BasePage {

  public slug = 'transfers/outgoingtrustdetails'

  public checkTrustDetails(trustInfo): this {

    cy.get('.govuk-summary-list').as('outgoingTrustInfo')

    cy.get('@outgoingTrustInfo').should('contain.text', trustInfo.name)
    cy.get('@outgoingTrustInfo').should('contain.text', trustInfo.companiesHouseNo)
    cy.get('@outgoingTrustInfo').should('contain.text', trustInfo.ukPrn)

    return this
  }

}

const outgoingTrustDetailsPage = new OutgoingTrustDetailsPage()

export default outgoingTrustDetailsPage
