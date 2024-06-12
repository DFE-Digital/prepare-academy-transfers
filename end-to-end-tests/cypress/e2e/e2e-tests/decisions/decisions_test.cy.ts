import { NewTransferProjectWithDecisions } from 'cypress/pages/newTransferProjectWithDecisions';

describe('Transfer Project Tests', () => {
  const transferProject = new NewTransferProjectWithDecisions();
  let projectId: string | undefined;

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
      projectId = url.match(/project\/(\d+)/)?.[1]; 
    });
    
    // Delete the project and verify that it was deleted successfully
    if (projectId) {
      transferProject.deleteProject(projectId);
    }
  });
});
