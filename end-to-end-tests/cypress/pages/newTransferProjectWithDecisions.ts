// newTransferProjectWithDecisions.ts

export class NewTransferProjectWithDecisions {
  visit(url: string) {
    cy.visit(url);
  }

  createNewTransferProject() {
    cy.get('[data-test="create-transfer"]').click();
    cy.contains('button.govuk-button', 'Create a new transfer').click();
  }

  searchOutgoingTrust(trustName: string) {
    cy.get('#SearchQuery').type(trustName);
    cy.get('button.govuk-button').contains('Search').click();
  }

  selectOutgoingTrust(trustId: string) {
    cy.get(`#${trustId}`).check();
    cy.get('button.govuk-button').contains('Continue').click();
  }
createProjectButton() {
  cy.get('[data-test="create-project"]').click();
  }
  confirmOutgoingTrust() {
    cy.get('[data-test="confirm-outgoing-trust"]').click();
  }
  selectOptionById(optionId: string) {
    cy.get(`#${optionId}`).click();
  }
  submitForm() {
    cy.get('[type="submit"]').click();
  }

  submitFormRecordDecision(){
    cy.get('#submit-btn').click();
  }
  recordDecision() {
    cy.contains('Record a decision').click();
    cy.get('#record-decision-link').click();
  }

  makeDecision(decisionId: string) {
    cy.get(`#${decisionId}-radio`).click();
    cy.get('#submit-btn').click();
  }

  enterDecisionMakerName(name: string) {
    cy.get('#decision-maker-name').type(name);
    cy.get('#submit-btn').click();
  }

  addPerformanceConcerns(concernsText: string) {
    cy.get('#performanceconcerns-checkbox').check();
    cy.get('#performanceconcerns-txtarea').type(concernsText);
    cy.get('#submit-btn').click();
  }

  enterDecisionDate(day: string, month: string, year: string) {
    cy.get('#decision-date-day').type(day);
    cy.get('#decision-date-month').type(month);
    cy.get('#decision-date-year').type(year);
    cy.get('#submit-btn').click();
  }

  verifyDecisionDetails() {
    cy.contains('Record a decision').click();
    cy.get('#decision').should('contain', 'Deferred');
    cy.get('#decision-made-by').should('contain', 'Deputy Director');
    cy.get('#deferred-reasons').should('contain', 'Performance concerns:');
    cy.get('#deferred-reasons').should('contain', 'Cypress Test');
    cy.get('#decision-date').should('contain', '12 December 2023');
  }

    // Assertion methods
    assertTrustName(expectedName: string) {
      cy.get('[data-cy="trust_Name"]').should('contain', expectedName);
    }
    
    assertURNNumber(expectedNumber: string) {
      cy.get('[data-cy="URN_Id"]').should('contain', expectedNumber);
    }
}
