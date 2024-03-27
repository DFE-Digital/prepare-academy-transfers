import { BasePage } from "./basePage"

class ProjectAssignmentPage extends BasePage{

  public slug = 'project-assignment'

  public assignDeliveryOfficer(deliveryOfficer): this {

    cy.get('[id="delivery-officer"]').type(deliveryOfficer)

    cy.get('li').contains(deliveryOfficer).click()

    this.continue()

    return this
  }

  public unassignDeliveryOfficer(): this {

    cy.get('[id="unassign-link"]').click()

    return this
  }
}

const projectAssignmentPage = new ProjectAssignmentPage()

export default projectAssignmentPage
