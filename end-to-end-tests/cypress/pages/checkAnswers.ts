import { BasePage } from './basePage';

class CheckAnswersPage extends BasePage {

  public slug = 'transfers/checkyouranswers';

  public checkDetails(outgoingTrust, incomingTrust): this {

    cy.get('.govuk-grid-column-full').as('trustAcademiesDetails');

    cy.get('@trustAcademiesDetails').then($el => {
      const textContent = $el.text().trim();
      
      expect(textContent).to.contain(outgoingTrust.name.trim());
      expect(textContent).to.contain(outgoingTrust.ukPrn.trim());
      expect(textContent).to.contain(outgoingTrust.academies.trim());
      expect(textContent).to.contain(incomingTrust.name.trim());
      
    });

    return this;
  }

  public continue(): this {
    cy.get('button.govuk-button').contains('Continue').click();
    return this;
  }

}

const checkAnswersPage = new CheckAnswersPage();

export default checkAnswersPage;