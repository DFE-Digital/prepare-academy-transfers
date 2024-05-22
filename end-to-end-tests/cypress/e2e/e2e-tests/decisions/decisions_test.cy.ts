import { NewTransferProjectWithDecisions } from 'cypress/pages/newTransferProjectWithDecisions';

describe('Transfer Project Tests', () => {
  const transferProject = new NewTransferProjectWithDecisions();

  beforeEach(() => {
    transferProject.visit(Cypress.env('url'));
  });

  it('Creating a transfer project and checking Decision of that project', () => {
    transferProject.createNewTransferProject();
    transferProject.searchOutgoingTrust('Greater Manchester Academies Trust');
    transferProject.selectOutgoingTrust('10058252');
    transferProject.confirmOutgoingTrust();
    transferProject.selectOptionById('10030221');
    transferProject.submitForm();

    // Select the option (Is the result of this transfer the formation of a new trust?) with id "false", then click continue
    transferProject.selectOptionById('false');
    transferProject.submitForm();

    // Select the option (Is there a preferred trust for these academies?) with id "false", then click continue
    transferProject.selectOptionById('false');
    transferProject.submitForm();

    // Assertion for trust name and Companies House number
    transferProject.assertTrustName('Greater Manchester Academies Trust');
    transferProject.assertURNNumber('136105');

    // Click continue
    transferProject.createProjectButton();

    // Click on "Record a decision" button
    transferProject.recordDecision();

    // Click on "Deferred" radio button, then click submit
    transferProject.makeDecision('deferred');

    // Click on "Grade 6" radio button, then click submit
    transferProject.makeDecision('grade6');

    // Enter "Fahad Darwish" in the text field, then click continue
    transferProject.enterDecisionMakerName('Fahad Darwish');

    // Click on "Performance concerns" checkbox, enter "Cypress Test", then click submit
    transferProject.addPerformanceConcerns('Cypress Test');

    // Enter date and click submit
    transferProject.enterDecisionDate('12', '12', '2023');

    // Click submit
    transferProject.submitFormRecordDecision();

    // Click on "Record a decision" and verify the information matches
    transferProject.verifyDecisionDetails();
  });
});
