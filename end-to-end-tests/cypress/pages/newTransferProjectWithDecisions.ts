export class NewTransferProjectWithDecisions {
  public visit(url: string): this {
    cy.visit(url);
    return this;
  }

  public createNewTransferProject(): this {
    cy.get('[data-test="create-transfer"]').click();
    cy.contains('button.govuk-button', 'Create a new transfer').click();
    return this;
  }

  public searchOutgoingTrust(trustName: string): this {
    cy.get('#SearchQuery').type(trustName);
    cy.get('button.govuk-button').contains('Search').click();
    return this;
  }

  public selectOutgoingTrust(trustId: string): this {
    cy.get(`#${trustId}`).check();
    cy.get('button.govuk-button').contains('Continue').click();
    return this;
  }

  public createProjectButton(): this {
    cy.get('[data-test="create-project"]').click();
    return this;
  }

  public confirmOutgoingTrust(): this {
    cy.get('[data-test="confirm-outgoing-trust"]').click();
    return this;
  }

  public selectOptionById(optionId: string): this {
    cy.get(`#${optionId}`).click();
    return this;
  }

  public submitForm(): this {
    cy.get('[type="submit"]').click();
    return this;
  }

  public submitFormRecordDecision(): this {
    cy.get('#submit-btn').click();
    return this;
  }

  public recordDecision(): this {
    cy.contains('Record a decision').click();
    cy.get('#record-decision-link').click();
    return this;
  }

  public makeDecision(decisionId: string): this {
    cy.get(`#${decisionId}-radio`).click();
    cy.get('#submit-btn').click();
    return this;
  }

  public enterDecisionMakerName(name: string): this {
    cy.get('#decision-maker-name').type(name);
    cy.get('#submit-btn').click();
    return this;
  }

  public addPerformanceConcerns(concernsText: string): this {
    cy.get('#performanceconcerns-checkbox').check();
    cy.get('#performanceconcerns-txtarea').type(concernsText);
    cy.get('#submit-btn').click();
    return this;
  }

  public enterDecisionDate(day: string, month: string, year: string): this {
    cy.get('#decision-date-day').type(day);
    cy.get('#decision-date-month').type(month);
    cy.get('#decision-date-year').type(year);
    cy.get('#submit-btn').click();
    return this;
  }

  public verifyDecisionDetails(): this {
    cy.contains('Record a decision').click();
    cy.get('#decision').should('contain', 'Deferred');
    cy.get('#decision-made-by').should('contain', 'Deputy Director');
    cy.get('#deferred-reasons').should('contain', 'Performance concerns:');
    cy.get('#deferred-reasons').should('contain', 'Cypress Test');
    cy.get('#decision-date').should('contain', '12 December 2023');
    return this;
  }

  public assertTrustName(expectedName: string): this {
    cy.get('[data-cy="trust_Name"]').should('contain', expectedName);
    return this;
  }

  public assertURNNumber(expectedNumber: string): this {
    cy.get('[data-cy="URN_Id"]').should('contain', expectedNumber);
    return this;
  }

  public deleteProject(projectId: string): this {
    const deleteUrl = `https://dev.prepare-transfers.education.gov.uk/project/${projectId}/delete`;
    cy.request('DELETE', deleteUrl).then((response) => {
      expect(response.status).to.eq(200); // Verify the response status
    });
    return this;
  }

}
