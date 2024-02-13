import checkAnswersPage from "cypress/pages/checkAnswers"
import homePage from "cypress/pages/home"
import incomingTrustSearchPage from "cypress/pages/incomingTrustSearch"
import incomingTrustSearchResultsPage from "cypress/pages/incomingTrustSearchResults"
import newTransferPage from "cypress/pages/newTransfer"
import outgoingTrustAcademiesPage from "cypress/pages/outgoingTrustAcademies"
import outgoingTrustDetailsPage from "cypress/pages/outgoingTrustDetailsPage"
import outgoingTrustSearchPage from "cypress/pages/outgoingTrustSearch"
import outgoingTrustSearchResultsPage from "cypress/pages/outgoingTrustSearchResults"
import projectPage from "cypress/pages/project"
import schoolDataPage from "cypress/pages/schoolData"

describe('School Data', () => {

  let outgoingTrustData, incomingTrustData, projectId
  
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

      projectId = projectPage.getProject()
  })

  beforeEach(() => {
    cy.visit(`${Cypress.env('url')}project/${projectId}`)

    projectPage
      .viewSchoolData()
  })

  it('Shows General Information', () => {

    schoolDataPage
      .checkGeneralInformation()
      .confirm()
  })

  it('Shows Pupil Numbers', () => {

    schoolDataPage
      .checkPupilNumbers()
      .confirm()
  })

  it('Shows Latest Ofsted Report', () => {

    schoolDataPage
      .checkOfstedReport()
      .confirm()
  })

  it('Shows KS4 Performance Tables', () => {

    schoolDataPage
      .checkKS4Tables()
      .confirm()
  })

  it('Shows KS5 Performance Tables', () => {

    schoolDataPage
      .checkKS5Tables()
      .confirm()
  })
})