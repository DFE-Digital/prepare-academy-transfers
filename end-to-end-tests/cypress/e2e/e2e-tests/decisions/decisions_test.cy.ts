import { NewTransferProjectWithDecisions } from 'cypress/pages/newTransferProjectWithDecisions';

describe('Transfer Project Tests', () => {
  const transferProject = new NewTransferProjectWithDecisions();
  let projectId: string;

  beforeEach(() => {
    transferProject.visit(Cypress.env('url'));
  });

  it('Creating a transfer project and checking Decision of that project', () => {
    transferProject.createNewTransferProject()
      .searchOutgoingTrust('Greater Manchester Academies Trust')
      .selectOutgoingTrust('10058252')
      .confirmOutgoingTrust()
      .selectOptionById('10030221')
      .submitForm()
      .selectOptionById('false')
      .submitForm()
      .selectOptionById('false')
      .submitForm()
      .assertTrustName('Greater Manchester Academies Trust')
      .assertURNNumber('136105')
      .createProjectButton()
      .recordDecision()
      .makeDecision('deferred')
      .makeDecision('grade6')
      .enterDecisionMakerName('Fahad Darwish')
      .addPerformanceConcerns('Cypress Test')
      .enterDecisionDate('12', '12', '2023')
      .submitFormRecordDecision()
      .verifyDecisionDetails();

      // Capture the projectId dynamically from the URL
    cy.url().then((url) => {
      const match = url.match(/project\/(\d+)/);
      if (match && match[1]) {
        projectId = match[1]; 

        // Delete the project and verify that it was deleted successfully
        transferProject.deleteProject(projectId);
      } else {
        throw new Error('Project ID not found in the URL');
      }
    });
  });
});
