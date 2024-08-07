class DatesPage {

  public slug = 'features'

  public completeAdvisoryBoardDate(date): this {

    cy.get('[data-test="ab-date"]').click()

    cy.get('h1').should('contain.text', 'Advisory board date')


    cy.get('[data-test="day"]').clear()
    cy.get('[data-test="day"]').type(date.date())
    cy.get('[data-test="month"]').clear()
    cy.get('[data-test="month"]').type(date.month() + 1)
    cy.get('[data-test="year"]').clear()
    cy.get('[data-test="year"]').type(date.year())

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(0).should('contain.text', date.format('D MMMM YYYY'))

    return this
  }

  public completeExpectedTransferDate(date): this {

    cy.get('[data-test="target-date"]').click()

   

    cy.get('[data-test="month"]').clear()
    cy.get('[data-test="month"]').type(date.month() + 1)
    cy.get('[data-test="year"]').clear()
    cy.get('[data-test="year"]').type(date.year())

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(4).should('contain.text', date.format('1 MMMM YYYY'))

    return this
  }

  public confirmDates(): this {

    cy.get('button').contains('Confirm and continue').click()

    return this
  }
}

const datesPage = new DatesPage()

export default datesPage
