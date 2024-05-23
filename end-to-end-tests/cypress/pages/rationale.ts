class RationalePage {

  public slug = 'rationale'

  public completeRationale(): this {

    cy.get('[data-test="project-rationale"]').click()    

    cy.get('h1').should('contain.text', 'Write the rationale for the project')

    cy.get('[data-test="project-rationale"]').clear()
    cy.get('[data-test="project-rationale"]').type('Cypress project rationale')

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(0).should('contain.text', 'Cypress project rationale')

    return this
  }

  public completeChosenReason(): this {

    cy.get('[data-test="trust-rationale"]').click()

    cy.get('h1').should('contain.text', 'Write the rationale for the incoming trust or sponsor')

    cy.get('[data-test="trust-rationale"]').clear()
    cy.get('[data-test="trust-rationale"]').type('Cypress trust rationale')

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(2).should('contain.text', 'Cypress trust rationale')

    return this
  }

  public markAsComplete(): this {

    cy.get('[data-test="mark-section-complete"]').click()
    return this
  }

  public confirmRationale(): this {

    cy.get('button').contains('Confirm and continue').click()

    return this
  }
}

const rationalePage = new RationalePage()

export default rationalePage
