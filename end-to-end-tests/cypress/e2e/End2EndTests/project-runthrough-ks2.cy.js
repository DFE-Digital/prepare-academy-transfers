/// <reference types ='Cypress'/>

let selectExistingAcademyTransfer = (projectName) =>
    cy.get(`.govuk-link--no-visited-state`).then(projects => {
        for (let i = 0; i < projects.length; i++) {
            if (projects[i].text.includes("The Hermitage School")) {
                projects[i].click();
                break;
            }
        }
})

let submit = () =>
    cy.get("[type='submit']")
        .click();

function clickBackLink() {
    cy.get(".govuk-back-link")
        .click();
}


describe("Locate project and check against Key Stage 2 table", { tags: '@skip'}, () => {
    afterEach(() => {
        cy.storeSessionData()
    });

    beforeEach(function () {
        cy.login()
    })

    let projectUrl = ""

    // *** School performance *** 
    // Key stage 2 performance tables
    it.skip('Locate Key Stage 2', () => {
        selectExistingAcademyTransfer("The Hermitage School");
        cy.get('[data-test="sd-academy-1"]').click()
        cy.get(`[data-test='ks2-performance']`).click()
        clickBackLink();
        cy.get(`[data-test='ks2-performance']`).click()
        cy.get(`[data-test='additional-information']`).click()
        clickBackLink();
        cy.get(`[data-test='ks2-performance']`).click()
        cy.get(`[data-test='additional-information']`).click()
        cy.fillInText("AdditionalInformation", "Additional information for ks2 performance");
        submit();
        clickBackLink()
    })
})