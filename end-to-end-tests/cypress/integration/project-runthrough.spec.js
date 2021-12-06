const fs = require("fs");
let getDataTest = (dataTest) => cy.get(`[data-test='${dataTest}']`)

let selectFirstRadio = () => {
    cy.get("[type='radio']").then(options => {
        let option = options[0];
        option.click();
    });
}

let submit = () =>
    cy.get("[type='submit']")
        .click();

function clickDataTest(dataTest) {
    getDataTest(dataTest)
        .click();
}

function login() {
    cy.get("[name='username']")
        .type("username")
        .should("have.value", "username");

    cy.get("[name='password']")
        .type("password")
        .should("have.value", "password");

    submit();
}

function createNewTransfer() {
    cy.get("[data-test='create-transfer']")
        .click()
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
        // cy.visit("/")
        // login();
        createNewTransfer();
        searchForTrustWithQuery("bishop fraser")
        selectFirstRadio();
        submit();
        clickDataTest("confirm-outgoing-trust")
        cy.selectRadio(2);
        submit();
        searchForTrustWithQuery("burnt");
        selectFirstRadio();
        submit();
        clickDataTest("create-project")

        // Features
        clickDataTest("transfer-features")
        clickDataTest("initiated")
        selectFirstRadio()
        submit();
        clickDataTest("reason")
        selectFirstRadio();
        cy.fillInText("moreDetail", "Some more detail and reasons")
        submit()
        clickDataTest("type")
        selectFirstRadio();
        submit()
        cy.clickBackLink();

        // Dates
        clickDataTest("transfer-dates")
        clickDataTest("first-discussed")
        cy.fillInDate("12", "12", "2020")
        submit();
        clickDataTest("target-date")
        cy.fillInDate("12", "12", "2020")
        submit();
        clickDataTest("htb-date")
        cy.fillInDate("12", "12", "2020")
        submit();
        clickBackLink()

        // Benefits
        clickDataTest("transfer-benefits")
        clickDataTest("intended-benefits")
        cy.selectCheckbox(0);
        cy.selectCheckbox(1);
        submit();
        clickDataTest("other-factors")
        cy.selectCheckbox(0);
        cy.fillInTextAtIndex(0, "First")
        cy.selectCheckbox(1);
        cy.fillInTextAtIndex(1, "second")
        submit();
        clickBackLink()

        //Rationale
        clickDataTest("transfer-rationale")
        clickDataTest("project-rationale")
        cy.fillInText("ProjectRationale", "this is the project rationale")
        submit();
        clickDataTest("trust-rationale")
        cy.fillInText("TrustOrSponsorRationale", "this is the project rationale")
        submit();
        clickBackLink()

        // Academy trust information
        clickDataTest("academy-trust-information")
        clickDataTest("recommendation");
        selectFirstRadio();
        cy.fillInText("author", "Author name")
        submit();
        clickBackLink()

        clickDataTest("general-information")
        clickBackLink()

        // Pupil numbers
        clickDataTest("pupil-numbers")
        clickDataTest("additional-information")
        cy.fillInText("additionalInformation", "Additional information for pupil numbers");
        submit();
        clickBackLink()

        // Ofsted
        clickDataTest("ofsted")
        clickDataTest("additional-information")
        cy.fillInText("additionalInformation", "Additional information for ofsted");
        submit();
        clickBackLink()

        // KS2
        clickDataTest("ks2-performance")
        clickDataTest("additional-information")
        cy.fillInText("additionalInformation", "Additional information for ks2 performance");
        submit();
        clickBackLink()

        // KS4
        clickDataTest("ks4-performance")
        clickDataTest("additional-information")
        cy.fillInText("additionalInformation", "Additional information for ks4 performance");
        submit();
        clickBackLink()

        // KS5
        clickDataTest("ks5-performance")
        clickDataTest("additional-information")
        cy.fillInText("additionalInformation", "Additional information for ks5 performance");
        submit();
        clickBackLink()
        
        // Go to preview
        clickDataTest("preview-htb")
        clickBackLink()

        clickDataTest("generate-htb")
        cy.get("[data-test='download-htb']").should($a => {
            expect($a.attr('href'), 'href').to.include("/advisory-board")
            expect($a.attr('target'), 'target').to.equal('_blank')
        })

        cy.location().then(location => {
            let currentUrn = location.pathname.split('/')[2]
            console.log(currentUrn);
            cy.request(`/project/${currentUrn}/advisory-board`).then(response => {
                expect(response.status).to.equal(200)
                expect(response.headers['content-type']).to.equal("application/vnd.openxmlformats-officedocument.wordprocessingml.document")
            })
        })

    });
    after(function () {
        cy.clearLocalStorage();
    });
});
