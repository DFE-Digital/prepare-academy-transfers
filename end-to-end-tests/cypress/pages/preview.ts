class PreviewPage {

  public slug = 'advisory-board/preview'

  // Checks the section counts
  // Note: Does *not* check contents are valid
  public checkSections(): this {

    cy.get('h1').should('contain.text', 'Preview project template')

    cy.get('h2.govuk-heading-l').then(($headings) => {
      cy.wrap($headings).should('have.length.at.least', 3)
      cy.wrap($headings[0]).should('contain.text', 'Transfer details')
      cy.wrap($headings[$headings.length -1]).should('contain.text', 'Generate project template')
    })

    cy.get('section').then(($sections) => {
      // Key Stage Data may vary so set minimum
      cy.wrap($sections).should('have.length.at.least', 9)
    })

    return this
  }
}

const previewPage = new PreviewPage()

export default previewPage
