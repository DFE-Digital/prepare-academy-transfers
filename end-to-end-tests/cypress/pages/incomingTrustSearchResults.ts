import { BasePage } from './basePage'

class IncomingTrustSearchResultsPage extends BasePage {

  public slug = 'transfers/searchincomingtrust'

  public selectTrust(trustName): this {

    cy.get('label').contains(trustName).click()

    this.continueTransfer()

    return this
  }
}

const incomingTrustSearchResultsPage = new IncomingTrustSearchResultsPage()

export default incomingTrustSearchResultsPage
