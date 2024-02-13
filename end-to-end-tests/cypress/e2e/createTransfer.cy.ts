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

describe('Create a new transfer', () => {

  let outgoingTrustData, incomingTrustData;

  beforeEach(() => {
    cy.visit(Cypress.env('url'))

    cy.fixture('trustInformation.json').then((jsonData) => {
      outgoingTrustData = jsonData[0]
      // Only single academy required
      outgoingTrustData.academies = outgoingTrustData.academies[0]
      
      incomingTrustData = jsonData[1]
    })
  })

  it('Creates a new academy transfer', () => {

    homePage
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

    incomingTrustSearchPage
      .searchTrustsByName(incomingTrustData.name)

    incomingTrustSearchResultsPage
      .selectTrust(incomingTrustData.name)

    checkAnswersPage
      .checkDetails(outgoingTrustData, incomingTrustData)
      .continue()

    cy.url().should('include', `${projectPage.slug}`)
  })
})