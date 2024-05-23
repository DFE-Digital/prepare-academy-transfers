class FeaturesPage {
  public slug = 'features';

  public completeReasonForTransfer(): this {
    // Click the first matched element
    cy.get('[data-test="initiated"]').first().click();

    cy.get('h1').should('contain.text', 'What is the reason for this transfer?');

    // Check the labels of the radios
    cy.get('.govuk-radios__item').then(($reasons) => {
      cy.wrap($reasons).should('have.length', 3);

      cy.wrap($reasons[0]).should('contain.text', 'Intervention');
      cy.wrap($reasons[1]).should('contain.text', 'Initiated by trust');
      cy.wrap($reasons[2]).should('contain.text', 'Sponsor or trust closure');
    });

    cy.get('[value="Dfe"]').click();

    cy.get('button').contains('Save and continue').click();

    cy.get('#Finance').click();
cy.get("button").contains("Save and continue").click();

    // Check the table has been updated
    cy.get('dd').eq(0).should('contain.text', 'Intervention');

    return this;
  }

  public completeTypeOfTransfer(): this {
    // Click the first matched element
    cy.get('[data-test="type"]').first().click();

    cy.get('h1').should('contain.text', 'What type of transfer is it?');

    // Check the labels of the radios
    cy.get('.govuk-radios__item').then(($types) => {
      cy.wrap($types).should('have.length', 6);

      cy.wrap($types[0]).should('contain.text', 'Closure of a SAT and the academy joining a MAT');
      cy.wrap($types[1]).should('contain.text', 'Closure of a MAT and academies joining a MAT');
      cy.wrap($types[2]).should('contain.text', 'Academy moving out of a MAT and joining another MAT');
      cy.wrap($types[3]).should('contain.text', 'Academies joining together to form a new MAT');
      cy.wrap($types[4]).should('contain.text', 'Closure of one of more MAT(s) to form a new MAT');
      cy.wrap($types[5]).should('contain.text', 'Any other type of transfer');
    });

    cy.get('[value="SatClosure"]').click();

    cy.get('button').contains('Save and continue').click();

    // Check the table has been updated
    cy.get('dd').eq(4).should('contain.text', 'Closure of a SAT and the academy joining a MAT');

    return this;
  }

  public markAsComplete(): this {
    cy.get('[data-test="mark-section-complete"]').click();
    return this;
  }

  public confirmFeatures(): this {
    cy.get('button').contains('Confirm and continue').click();
    return this;
  }
}

const featuresPage = new FeaturesPage();

export default featuresPage;
