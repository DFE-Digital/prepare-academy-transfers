class TrustInformationProjectDatesPage {

  public slug = 'academy-and-trust-information'

  // Whilst two separate links on the page, both of these are on the same form so are merged here
  public completeRecommendationAndAuthor(): this {

    cy.get('[data-test="recommendation"]').click()    

    cy.get('h1').should('contain.text', 'Recommendation')

    cy.get('legend').should('contain.text', 'What is the recommendation for this project?')

    // Check the labels of the radios
    cy.get('.govuk-radios__item').then(($recommendations) => {
      cy.wrap($recommendations).should('have.length', 2)

      cy.wrap($recommendations[0]).should('contain.text', 'Approve')
      cy.wrap($recommendations[1]).should('contain.text', 'Decline')
    })

    cy.get('[value="Approve"]').click()

    cy.get('label[for="author"]').should('contain.text', 'Enter the full name of the author of this project template')
    cy.get('input[id="author"]').clear()
    cy.get('input[id="author"]').type('Chris Sherlock')

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(0).should('contain.text', 'Approve')
    cy.get('dd').eq(2).should('contain.text', 'Chris Sherlock')

    return this
  }

  public checkOtherTableData(advisoryBoardDate, projectName, transferDate): this {
    
    cy.get('[data-test="transfer-dates"]').click()
    cy.get('dd').eq(0).should('contain.text', advisoryBoardDate.format('D MMMM YYYY'))
    cy.get('[data-test="trust_name"]').should('contain.text', projectName)
    cy.get('dd').eq(4).should('contain.text', transferDate.format('1 MMMM YYYY'))
    

    return this
  }

  public confirmTrustInformationProjectDates(): this {

    cy.get('button').contains('Confirm and continue').click()

    return this
  }
}

const trustInformationProjectDatesPage = new TrustInformationProjectDatesPage()

export default trustInformationProjectDatesPage
