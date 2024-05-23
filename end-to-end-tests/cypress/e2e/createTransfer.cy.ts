import homePage from '../pages/home'
import newTransferPage from 'cypress/pages/newTransfer'
import outgoingTrustSearchPage from 'cypress/pages/outgoingTrustSearch'
import outgoingTrustSearchResultsPage from 'cypress/pages/outgoingTrustSearchResults'
import outgoingTrustDetailsPage from 'cypress/pages/outgoingTrustDetailsPage'
import outgoingTrustAcademiesPage from 'cypress/pages/outgoingTrustAcademies'
import incomingTrustSearchPage from 'cypress/pages/incomingTrustSearch'
import checkAnswersPage from 'cypress/pages/checkAnswers'
import projectPage from 'cypress/pages/project'
import benefitsPage from 'cypress/pages/benefits'
import datesPage from 'cypress/pages/dates'
import downloadPage from 'cypress/pages/download'
import featuresPage from 'cypress/pages/features'
import legalRequirementsPage from 'cypress/pages/legalRequirements'
import previewPage from 'cypress/pages/preview'
import projectAssignmentPage from 'cypress/pages/projectAssignment'
import rationalePage from 'cypress/pages/rationale'
import trustInformationProjectDatesPage from 'cypress/pages/trustInformationProjectDates'

describe('Create a new transfer', () => {

  let outgoingTrustData, incomingTrustData, projectId
  const advisoryBoardDate = Cypress.dayjs().add(3, 'month')
  const transferDate = Cypress.dayjs().add(4, 'month')

  beforeEach(() => {

    cy.fixture('trustInformation.json').then((jsonData) => {
      outgoingTrustData = jsonData[0]
      // Only single academy required
      outgoingTrustData.academies = outgoingTrustData.academies[0]
      
      incomingTrustData = jsonData[1]
    })
  })

  context('Create new transfer', () => {
    it('Creates a new academy transfer', () => {

      homePage
        .open()
        .startCreateNewTransfer()

      newTransferPage
        .clickCreateNewTransfer()

      outgoingTrustSearchPage
        .searchTrustsByName(outgoingTrustData.name)

      outgoingTrustSearchResultsPage
        .selectTrust(outgoingTrustData.name)

      outgoingTrustDetailsPage
        .checkTrustDetails(outgoingTrustData)
        .continue()

      outgoingTrustAcademiesPage.selectSingleAcademy(outgoingTrustData.academies)
        .continue()

          // Select the option (Is the result of this transfer the formation of a new trust?) with id "false", then click continue
          outgoingTrustAcademiesPage.selectOptionYes();
          outgoingTrustAcademiesPage.submitForm();

   
      incomingTrustSearchPage
        .searchTrustsByName(incomingTrustData.name)


      checkAnswersPage
        .checkDetails(outgoingTrustData, incomingTrustData)
        .continue()

      cy.url().then(($url) =>{
        cy.wrap($url).should('include', `${projectPage.slug}`)
        projectId = $url.split('/').pop()
      })
    })
  })

  context('Project information', () => {
    it('Has the Project Reference and School Name', () => {

      projectPage
        .loadProject(projectId)
        .checkProjectId(projectId)
        .checkSchoolName(incomingTrustData.name)
    })
  })

  context('Delivery Officer', () => {
    it('Assign and Unassign Delivery Officer', () => {

      const deliveryOfficer = 'Chris Sherlock'

      projectPage
        .loadProject(projectId)
        .checkDeliveryOfficerDetails('Empty')
        .startChangeDeliveryOfficer()

      projectAssignmentPage
        .assignDeliveryOfficer(deliveryOfficer)

      projectPage
        .checkDeliveryOfficerAssigned(deliveryOfficer)
        .startChangeDeliveryOfficer()

      projectAssignmentPage
        .unassignDeliveryOfficer()

      projectPage
        .checkDeliveryOfficerAssigned('Empty')
    })
  })

  context('Completing details', () => {
    it('Fill in Features', () => {

      projectPage
        .loadProject(projectId)
        .checkFeaturesStatus('Not Started')
        .startTransferFeatures()

      featuresPage
        .completeReasonForTransfer()
        .completeTypeOfTransfer()
        .markAsComplete()
        .confirmFeatures()

      projectPage
        .checkFeaturesStatus('Completed')
    })

    it('Fill in Transfer Dates', () => {

      cy.visit(`${Cypress.env('url')}project/${projectId}`)

      projectPage
        .checkTransferDatesStatus('Not Started')
        .startTransferDates()

      datesPage
        .completeAdvisoryBoardDate(advisoryBoardDate)
        .completeExpectedTransferDate(transferDate)
        .confirmDates()

      projectPage
        .checkTransferDatesStatus('Completed')
    })

    it('Fill in Benefits and Risks', () => {

      cy.visit(`${Cypress.env('url')}project/${projectId}`)

      projectPage
        .loadProject(projectId)
        .checkBenefitsAndRiskStatus('Not Started')
        .startBenefitsAndRisk()

      benefitsPage
        .completeBenefits()
        .completeRisks()
        .completeEqualitiesImpactAssessment()
        .markAsComplete()
        .confirmBenefitsRisks()

      projectPage
        .checkBenefitsAndRiskStatus('Completed')
    })

    it('Fill in Legal Requirements', () => {

      cy.visit(`${Cypress.env('url')}project/${projectId}`)

      projectPage
        .loadProject(projectId)
        .checkLegalRequirementsStatus('Not Started')
        .startLegalRequirements()

      legalRequirementsPage
        .completeResolution()
        .completeAgreement()
        .completeDiocesanConsent()
        .markAsComplete()
        .confirmLegalRequirements()

      projectPage
        .checkLegalRequirementsStatus('Completed')
    })

    it('Fill in Rationale', () => {

      cy.visit(`${Cypress.env('url')}project/${projectId}`)

      projectPage
        .loadProject(projectId)
        .checkRationaleStatus('Not Started')
        .startRationale()

      rationalePage
        .completeRationale()
        .completeChosenReason()
        .markAsComplete()
        .confirmRationale()

      projectPage
        .checkRationaleStatus('Completed')
    })

    it('Fill in Trust Information and Project Dates', () => {
      projectPage
        .loadProject(projectId)
        .checkTrustInformationProjectDatesStatus('Not Started')
        .startTrustInformationProjectDates()

      trustInformationProjectDatesPage
        .completeRecommendationAndAuthor()
        .checkOtherTableData(advisoryBoardDate, incomingTrustData.name, transferDate)
        .confirmTrustInformationProjectDates()

      projectPage
        .checkTrustInformationProjectDatesStatus('Completed')
    })
  })

  context('Project Template', () => {
    it('Preview Project Template', () => {

      projectPage
        .loadProject(projectId)
        .openPreviewProjectTemplate()

      previewPage
        .checkSections()
    })

    it('Generate Project Template', () => {

      projectPage
        .loadProject(projectId)
        .generateProjectTemplate()

      downloadPage
        .downloadProjectTemplate()
    })
  })
})
