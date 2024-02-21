class LegalRequirementsPage {

  public slug = 'legalrequirements'

  public completeResolution(): this {

    cy.get('a[href*="outgoing-trust-consent"]').click()    

    cy.get('h1').should('contain.text', 'Have you received a resolution from the outgoing trust?')

    // Check the labels of the radios
    cy.get('.govuk-radios__item').then(($resolutions) => {
      cy.wrap($resolutions).should('have.length', 3)

      cy.wrap($resolutions[0]).should('contain.text', 'Yes')
      cy.wrap($resolutions[1]).should('contain.text', 'No')
      cy.wrap($resolutions[2]).should('contain.text', 'Not Applicable')
    })

    cy.get('[value="Yes"]').click()

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(0).should('contain.text', 'Yes')

    return this
  }

  public completeAgreement(): this {

    cy.get('a[href*="incoming-trust-agreement"]').click()

    cy.get('h1').should('contain.text', 'Has the incoming trust agreed to take on the academy?')

    // Check the labels of the radios
    cy.get('.govuk-radios__item').then(($resolutions) => {
      cy.wrap($resolutions).should('have.length', 3)

      cy.wrap($resolutions[0]).should('contain.text', 'Yes')
      cy.wrap($resolutions[1]).should('contain.text', 'No')
      cy.wrap($resolutions[2]).should('contain.text', 'Not Applicable')
    })

    cy.get('[value="Yes"]').click()

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(2).should('contain.text', 'Yes')

    return this
  }

  public completeDiocesanConsent(): this {

    cy.get('a[href*="diocesan-consent"]').click()

    cy.get('h1').should('contain.text', 'Have you spoken with the diocese about the incoming trust?')

    // Check the labels of the radios
    cy.get('.govuk-radios__item').then(($resolutions) => {
      cy.wrap($resolutions).should('have.length', 3)

      cy.wrap($resolutions[0]).should('contain.text', 'Yes')
      cy.wrap($resolutions[1]).should('contain.text', 'No')
      cy.wrap($resolutions[2]).should('contain.text', 'Not Applicable')
    })

    cy.get('[value="Yes"]').click()

    cy.get('button').contains('Save and continue').click()

    // Check the table has been updated
    cy.get('dd').eq(4).should('contain.text', 'Yes')

    return this
  }

  public markAsComplete(): this {

    cy.get('[data-test="mark-section-complete"]').click()
    return this
  }

  public confirmLegalRequirements(): this {

    cy.get('button').contains('Confirm and continue').click()

    return this
  }
}

const legalRequirementsPage = new LegalRequirementsPage()

export default legalRequirementsPage
