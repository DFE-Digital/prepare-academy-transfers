const fs = require("fs");

let selectFirstRadio = () => {
    cy.get("[type='radio']").then(options => {
        let option = options[0];
        option.click();
    });
}

let submit = () =>
    cy.get("[type='submit']")
        .click();

function login() {
    cy.getDataTest("username")
        .type("username")
        .should("have.value", "username");

    cy.getDataTest("password")
        .type("password")
        .should("have.value", "password");

    submit();
}

function createNewTransfer() {
    cy.clickDataTest("create-transfer")     
}

function searchForTrustWithQuery(queryString) {
    cy.get("[name='query']")
        .type(queryString)
    submit();
}

function clickBackLink() {
    cy.get(".govuk-back-link")
        .click();
}

describe("Creating and editing an academy transfer", function () {

    afterEach(() => {
		cy.storeSessionData();
	});

	before(function () {
		cy.login();
	});

    it("Loads the page", function () {
        createNewTransfer();
        searchForTrustWithQuery("bishop fraser")
        selectFirstRadio();
        submit();
        cy.clickDataTest("confirm-outgoing-trust")
        cy.selectRadio(2);
        submit();
        searchForTrustWithQuery("burnt");
        selectFirstRadio();
        submit();
        cy.clickDataTest("create-project")

        // Features
        cy.clickDataTest("transfer-features")
        cy.clickDataTest("initiated")
        selectFirstRadio()
        submit();
        cy.clickDataTest("reason")
        selectFirstRadio();
        cy.fillInText("moreDetail", "Some more detail and reasons")
        submit()
        cy.clickDataTest("type")
        selectFirstRadio();
        submit()
        cy.clickBackLink();
        cy.getDataTest("features").should('have.text',"COMPLETED");

        // Dates
        cy.clickDataTest("transfer-dates")
        cy.clickDataTest("first-discussed")
        cy.fillInDate(Cypress.dayjs().add(-1,'M'))
        submit();
        cy.clickDataTest("target-date")
        cy.fillInDate(Cypress.dayjs().add(3,'M'))
        submit();
        cy.clickDataTest("ab-date")
        cy.fillInDate(Cypress.dayjs().add(2,'M'))
        submit();
        clickBackLink()
        cy.getDataTest("dates").should('have.text',"COMPLETED");
        // Benefits
        cy.clickDataTest("transfer-benefits")
        cy.clickDataTest("intended-benefits")
        cy.selectCheckbox(0);
        cy.selectCheckbox(1);
        submit();
        cy.clickDataTest("other-factors")
        cy.selectCheckbox(0);
        cy.fillInTextAtIndex(0, "First")
        cy.selectCheckbox(1);
        cy.fillInTextAtIndex(1, "second")
        submit();
        clickBackLink()
        cy.getDataTest("benefits").should('have.text',"COMPLETED");
        
        //Rationale
        cy.clickDataTest("transfer-rationale")
        cy.clickDataTest("project-rationale")
        cy.fillInText("ViewModel.ProjectRationale", "this is the project rationale")
        submit();
        cy.clickDataTest("trust-rationale")
        cy.fillInText("ViewModel.TrustOrSponsorRationale", "this is the project rationale")
        submit();
        clickBackLink()
        cy.getDataTest("rationale").should('have.text',"COMPLETED");
        
        // Academy trust information
        cy.clickDataTest("academy-trust-information")
        cy.clickDataTest("recommendation");
        selectFirstRadio();
        cy.fillInText("author", "Author name")
        submit();
        clickBackLink()
        cy.getDataTest("academyandtrustinformation").should('have.text',"COMPLETED");
        
        //School Data
        cy.clickDataTest("sd-academy-1")
        
        cy.clickDataTest("general-information")
        clickBackLink();
        
        // Pupil numbers
        cy.clickDataTest("pupil-numbers")
        cy.clickDataTest("additional-information")
        cy.fillInText("AdditionalInformation", "Additional information for pupil numbers");
        submit();
        clickBackLink();
        
        // Ofsted
        cy.clickDataTest("ofsted")
        cy.clickDataTest("additional-information")
        cy.fillInText("AdditionalInformation", "Additional information for ofsted");
        submit();
        clickBackLink()
        
        // KS2
        cy.clickDataTest("ks2-performance")
        cy.clickDataTest("additional-information")
        cy.fillInText("AdditionalInformation", "Additional information for ks2 performance");
        submit();
        clickBackLink()
        
        // KS4
        cy.clickDataTest("ks4-performance")
        cy.clickDataTest("additional-information")
        cy.fillInText("AdditionalInformation", "Additional information for ks4 performance");
        submit();
        clickBackLink()
        
        // KS5
        cy.clickDataTest("ks5-performance")
        cy.clickDataTest("additional-information")
        cy.fillInText("AdditionalInformation", "Additional information for ks5 performance");
        submit();
        clickBackLink()
        
        // Go to preview
        cy.clickDataTest("preview-htb")
        clickBackLink()

        cy.clickDataTest("generate-htb")
        cy.get("[data-test='download-htb']").should($a => {
            expect($a.attr('href'), 'href').to.include("/advisory-board")
            expect($a.attr('target'), 'target').to.equal('_blank')
        })

        cy.location().then(location => {
            let currentUrn = location.pathname.split('/')[2]
            console.log(currentUrn);
            cy.request(`/project/${currentUrn}/advisory-board/download/generatedocument`).then(response => {
                expect(response.status).to.equal(200)
                expect(response.headers['content-type']).to.equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            })
        })

    });
    after(function () {
        cy.clearLocalStorage();
    });
});
