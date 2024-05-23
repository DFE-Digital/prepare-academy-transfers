import { BasePage } from './basePage'

class OutgoingTrustAcademiesPage extends BasePage {

  public slug = 'transfers/outgoingtrustacademies'

  public selectMultipleAcademies(academies): this {

    academies.array.forEach(academy => {
      this.selectSingleAcademy(academy)
    })

    return this
  }

  public selectSingleAcademy(academy): this {

    cy.get('.govuk-checkboxes').contains(academy).click()

    return this
  }
  selectOptionYes() {
    cy.get('[data-test="true"]').click();
  }
  selectOptionById(optionId: string) {
    cy.get(`#${optionId}`).click();
  }
  submitForm() {
    cy.get('[type="submit"]').click();
  }


}

const outgoingTrustAcademiesPage = new OutgoingTrustAcademiesPage()

export default outgoingTrustAcademiesPage
