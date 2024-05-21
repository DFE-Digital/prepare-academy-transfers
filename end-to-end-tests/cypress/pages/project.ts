class ProjectPage {

  public slug = 'project'

  public loadProject(projectId): this {

    cy.visit(`${Cypress.env('url')}project/${projectId}`)
    
    return this
  }

  public checkProjectId(id): this {

    cy.get('.govuk-caption-l').should('have.text', `Project reference: SAT-${id}`)

    return this
  }

  public checkSchoolName(schoolName): this {

    cy.get('h1').should('contain.text', schoolName)

    return this
  }

  public checkDeliveryOfficerAssigned(deliveryOfficer): this {

    const status = deliveryOfficer != 'Empty' ? 'assigned' : 'unassigned'

    cy.get('[data-module="govuk-notification-banner"]').should('be.visible')
    cy.get('[data-module="govuk-notification-banner"]').should('contain.text', 'Done')
    cy.get('[data-module="govuk-notification-banner"]').should('contain.text', `Project is ${status}`)

    this.checkDeliveryOfficerDetails(deliveryOfficer)
    
    return this
  }

  public checkDeliveryOfficerDetails(deliveryOfficer): this {

    cy.get('[data-id="assigned-user"]').should('have.text', deliveryOfficer)

    if(deliveryOfficer == 'Empty') {
      cy.get('[data-id="assigned-user"]').should('have.class', 'empty')
    }
    else {
      cy.get('[data-id="assigned-user"]').should('not.have.class', 'empty')
    }

    return this
  }

  public startChangeDeliveryOfficer(): this {

    cy.get('a').contains('Change').click()

    return this
  }

  public checkFeaturesStatus(status): this {
    cy.get('[data-test="features"]').should('have.text', status.toUpperCase())

    return this
  }

  public startTransferFeatures(): this {
    cy.get('[data-test="transfer-features"]').click()

    return this
  }

  public checkTransferDatesStatus(status): this {
    cy.get('[data-test="dates"]').should('have.text', status.toUpperCase())

    return this
  }

  public startTransferDates(): this {
    cy.get('[data-test="transfer-dates"]').click()
    
    return this
  }

  public checkBenefitsAndRiskStatus(status): this {
    cy.get('[data-test="benefits"]').should('have.text', status.toUpperCase())

    return this
  }

  public startBenefitsAndRisk(): this {
    cy.get('[data-test="transfer-benefits"]').click()
    
    return this
  }

  public checkLegalRequirementsStatus(status): this {
    cy.get('[data-test="legal-requirements"]').should('have.text', status.toUpperCase())

    return this
  }

  public startLegalRequirements(): this {
    cy.get('[data-test="transfer-legal-requirements"]').click()
    
    return this
  }

  public checkRationaleStatus(status): this {
    cy.get('[data-test="rationale"]').should('have.text', status.toUpperCase())

    return this
  }

  public startRationale(): this {
    cy.get('[data-test="transfer-rationale"]').click()
    
    return this
  }

  public checkTrustInformationProjectDatesStatus(status): this {
    cy.get('[data-test="academyandtrustinformation"]').should('have.text', status.toUpperCase())

    return this
  }

  public startTrustInformationProjectDates(): this {
    cy.get('[data-test="academy-trust-information"]').click()
    
    return this
  }

  public openPreviewProjectTemplate(): this {

    cy.get('[data-test="preview-htb"]').click()

    return this
  }

  public generateProjectTemplate(): this {

    cy.get('[data-test="generate-htb"]').click()

    return this
  }

  public viewSchoolData(): this {

    cy.get('[data-test="sd-academy-1"]').click()
    
    return this
  }
}

const projectPage = new ProjectPage()

export default projectPage
