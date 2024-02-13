import homePage from '../pages/home'
import newTransferPage from 'cypress/pages/newTransfer'
import outgoingTrustSearchPage from 'cypress/pages/outgoingTrustSearch'
import outgoingTrustSearchResultsPage from 'cypress/pages/outgoingTrustSearchResults'
import outgoingTrustDetailsPage from 'cypress/pages/outgoingTrustDetailsPage'
import outgoingTrustAcademiesPage from 'cypress/pages/outgoingTrustAcademies'
import incomingTrustSearchPage from 'cypress/pages/incomingTrustSearch'
import incomingTrustSearchResultsPage from 'cypress/pages/incomingTrustSearchResults'
import checkAnswersPage from 'cypress/pages/checkAnswers'
import projectPage from 'cypress/pages/project'
import projectAssignmentPage from 'cypress/pages/projectAssignment'
import featuresPage from 'cypress/pages/features'
import datesPage from 'cypress/pages/dates'
import benefitsPage from 'cypress/pages/benefits'
import legalRequirementsPage from 'cypress/pages/legalRequirements'
import rationalePage from 'cypress/pages/rationale'
import trustInformationProjectDatesPage from 'cypress/pages/trustInformationProjectDates'
import downloadPage from 'cypress/pages/download'
import previewPage from 'cypress/pages/preview'

describe('Create a project template', () => {

  let outgoingTrustData, incomingTrustData, projectId;
  const advisoryBoardDate = Cypress.dayjs().add(3, 'month')
  const transferDate = Cypress.dayjs().add(4, 'month')

  before('Create transfer project', () => {
    cy.visit(Cypress.env('url'))

    cy.fixture('trustInformation.json').then((jsonData) => {
      outgoingTrustData = jsonData[0]
      // Only single academy required
      outgoingTrustData.academies = outgoingTrustData.academies[0]
      
      incomingTrustData = jsonData[1]
    })

    homePage
      .startCreateNewTransfer()
    newTransferPage
      .clickCreateNewTransfer()
    outgoingTrustSearchPage
      .searchTrustsByName(outgoingTrustData.name)
    outgoingTrustSearchResultsPage
      .selectTrust(outgoingTrustData.name)
    outgoingTrustDetailsPage
      .continue()
    outgoingTrustAcademiesPage
      .selectSingleAcademy(outgoingTrustData.academies)
      .continue()
    incomingTrustSearchPage
      .searchTrustsByName(incomingTrustData.name)
    incomingTrustSearchResultsPage
      .selectTrust(incomingTrustData.name)
    checkAnswersPage
      .continue()

    projectPage.getProject()
  })

  beforeEach(() => {
    cy.visit(`${Cypress.env('url')}project/${projectId}`)
  })

  it('Has the Project Reference and School Name', () => {
    
    projectPage
      .checkProjectId(projectId)
      .checkSchoolName(outgoingTrustData.academies)
  })

  it('Assign and Unassign Delivery Officer', () => {

    const deliveryOfficer = 'Chris Sherlock'

    projectPage
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

  it('Fill in Features', () => {
    projectPage
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
    projectPage
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
    projectPage
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
    projectPage
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
      .checkTrustInformationProjectDatesStatus('Not Started')
      .startTrustInformationProjectDates()

    trustInformationProjectDatesPage
      .completeRecommendationAndAuthor()
      .checkOtherTableData(advisoryBoardDate, outgoingTrustData.academies, transferDate)
      .confirmTrustInformationProjectDates()

    projectPage
      .checkTrustInformationProjectDatesStatus('Completed')
  })

  it('Preview Project Template', () => {

    projectPage
      .openPreviewProjectTemplate()

    previewPage
      .checkSections()
  })

  it('Generate Project Template', () => {
    
    projectPage
      .generateProjectTemplate()

    downloadPage
      .downloadProjectTemplate(projectId)
  })
})