import { BasePage } from './basePage'

class CheckAnswersPage extends BasePage {

  public slug = 'transfers/checkyouranswers'

  public checkDetails(outgoingTrust, incomingTrust): this {

    cy.get('.govuk-grid-column-full').as('trustAcademiesDetails')

    cy.get('@trustAcademiesDetails').should('contain.text', outgoingTrust.name)
    cy.get('@trustAcademiesDetails').should('contain.text', outgoingTrust.ukPrn)

    cy.get('@trustAcademiesDetails').should('contain.text', outgoingTrust.academies)

    cy.get('@trustAcademiesDetails').should('contain.text', incomingTrust.name)
    cy.get('@trustAcademiesDetails').should('contain.text', incomingTrust.ukPrn)

    return this
  }

}

const checkAnswersPage = new CheckAnswersPage()

export default checkAnswersPage
