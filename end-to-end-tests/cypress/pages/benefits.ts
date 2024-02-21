class BenefitsPage {

  public slug = 'benefits'

  public completeBenefits(): this {

    cy.get('[data-test="intended-benefits"]').click()

    cy.get('h1').should('contain.text', 'What are the intended benefits of the transfer?')

    // Check the labels of the checkboxes
    cy.get('.govuk-checkboxes__item').then(($benefits) => {
      cy.wrap($benefits).should('have.length', 9)

      cy.wrap($benefits[0]).should('contain.text', 'Stronger leadership')
      cy.wrap($benefits[1]).should('contain.text', 'Strengthening governance')
      cy.wrap($benefits[2]).should('contain.text', 'Improving safeguarding')
      cy.wrap($benefits[3]).should('contain.text', 'Secure financial position')
      cy.wrap($benefits[4]).should('contain.text', 'A central financial team and central support')
      cy.wrap($benefits[5]).should('contain.text', 'Improved pupil performance')
      cy.wrap($benefits[6]).should('contain.text', 'Improved Ofsted rating')
      cy.wrap($benefits[7]).should('contain.text', 'Long term stability')
      cy.wrap($benefits[8]).should('contain.text', 'Add another benefit')
    })

    cy.get('[id="Other"]').click()
    cy.get('[id="IntendedBenefitsViewModel_OtherBenefit"]').should('be.visible')
    cy.get('[id="IntendedBenefitsViewModel_OtherBenefit"]').type('Cypress benefit')

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(0).should('contain.text', 'Other: Cypress benefit')

    return this
  }

  public completeRisks(): this {

    cy.get('[data-test="any-risks"]').click()

    cy.get('h1').should('contain.text', 'Are there any risks to consider?')

    // Choose 'Yes' Radio button
    cy.get('[value="true"]').click()

    cy.get('button').contains('Save and continue').click()

    // Check the labels of the radios
    cy.get('.govuk-checkboxes__item').then(($risks) => {
      cy.wrap($risks).should('have.length', 4)

      cy.wrap($risks[0]).should('contain.text', 'Complex land and building issues')
      cy.wrap($risks[1]).should('contain.text', 'Finance and debt concerns')
      cy.wrap($risks[2]).should('contain.text', 'High profile transfer')
      cy.wrap($risks[3]).should('contain.text', 'Other risks')
    })

    cy.get('[data-test="OtherRisks"]').click()

    cy.get('button').contains('Save and continue').click()

    cy.get('h1').should('contain.text', 'What other risks are there? (optional)')

    cy.get('[data-test="high-profile-transfer"]').clear().type('Cypress risks')

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(2).should('contain.text', 'Yes')
    cy.get('dd').eq(4).should('contain.text', 'Cypress risks')

    return this
  }

  public completeEqualitiesImpactAssessment(): this {

    cy.get('a[data-test="equalities-impact-assessment"]').click()

    cy.get('h1').should('contain.text', 'Has an Equalities Impact Assessment been considered?')

    // Choose 'Yes' Radio button
    cy.get('[value="true"]').click()

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(6).should('contain.text', 'Yes')

    return this
  }

  public markAsComplete(): this {

    cy.get('[data-test="mark-section-complete"]').click()
    return this
  }

  public confirmBenefitsRisks(): this {

    cy.get('button').contains('Confirm and continue').click()

    return this
  }
}

const benefitsPage = new BenefitsPage()

export default benefitsPage
