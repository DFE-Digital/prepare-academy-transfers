const { clear } = require("console");
const fs = require("fs");

let selectFirstRadio = () => {
    cy.get("[type='radio']").then(options => {
        let option = options[0];
        option.click();
    });
}

let selectExistingAcademyTransfer = (projectName) =>
    cy.get(`.govuk-link--no-visited-state`).then(projects => {
        for(let i = 0; i < projects.length; i++){
            if(projects[i].text.includes("Burnt Ash Primary School")){
                projects[i].click();
                break;
            }
        }
    });

let submit = () =>
    cy.get("[type='submit']")
        .click();

function clickBackLink() {
    cy.get(".govuk-back-link")
        .click();
}

describe("Creating and editing an academy transfer", function () {

    afterEach(() => {
		cy.storeSessionData();
	});

	beforeEach(function () {
		cy.login();
	});

    let projectUrl = "";
    
    it("Create an Academy Transfer", function () {
        cy.login
        cy.visit("https://academy-transfers-dev.london.cloudapps.digital");

        cy.get("h1").should('contain.text',"Manage an academy transfer");
        /*
            cy.get('.govuk-button--start').should('contain.text', 'Start a new transfer project').click();
        
        cy.get("h1").should('contain.text',"What is the outgoing trust name?");
            cy.clickBackLink();

        cy.get("h1").should('contain.text',"Manage an academy transfer");
            cy.get('.govuk-button--start').should('contain.text', 'Start a new transfer project').click();

        cy.get("h1").should('contain.text',"What is the outgoing trust name?");
            cy.get("#SearchQuery").clear().type("bishop fraser")
            cy.get('.govuk-button').should('contain.text', 'Search').click();

        cy.get("h1").should('contain.text',"Select the outgoing trust");
            cy.clickBackLink();

        cy.get("h1").should('contain.text',"What is the outgoing trust name?");
            cy.get("#SearchQuery").clear().type("bishop fraser")
            cy.get('.govuk-button').should('contain.text', 'Search').click();

        cy.get("h1").should('contain.text',"Select the outgoing trust");
            selectFirstRadio()
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"Outgoing trust details");
            cy.clickBackLink();

        cy.get("h1").should('contain.text',"What is the outgoing trust name?");
            cy.get("#SearchQuery").clear().type("bishop fraser")
            cy.get('.govuk-button').should('contain.text', 'Search').click();

        cy.get("h1").should('contain.text',"Select the outgoing trust");
            selectFirstRadio()
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"Outgoing trust details");
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"Select the transferring academies");
            cy.selectCheckbox(0)
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"What is the incoming trust name?");
            cy.clickBackLink()

        cy.get("h1").should('contain.text',"Select the transferring academies");
            cy.selectCheckbox(0)
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"What is the incoming trust name?");
            cy.get("[name='query']").clear().type("burnt")
            cy.get('.govuk-button').should('contain.text', 'Search').click();

        cy.get("h1").should('contain.text',"Select an incoming trust");
            cy.clickBackLink()

        cy.get("h1").should('contain.text',"What is the incoming trust name?");
            cy.get("[name='query']").clear().type("burnt")
            cy.get('.govuk-button').should('contain.text', 'Search').click();
        
        cy.get("h1").should('contain.text',"Select an incoming trust");
            selectFirstRadio()
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"Check trust and academy details");
            cy.clickBackLink()
        
        cy.get("h1").should('contain.text',"What is the incoming trust name?");
            cy.get("[name='query']").clear().type("burnt")
            cy.get('.govuk-button').should('contain.text', 'Search').click();

        cy.get("h1").should('contain.text',"Select an incoming trust");
            selectFirstRadio()
            cy.get('.govuk-button').should('contain.text', 'Continue').click();   
    
        cy.get("h1").should('contain.text',"Check trust and academy details");
            cy.get('button[data-test="create-project"]').click()

        cy.get("h1").should('contain.text',"Burnt Ash Primary School");
        */
    });

    it.skip("Edit an Academy Transfer", function () {     
        cy.visit("https://academy-transfers-dev.london.cloudapps.digital/");   
        selectExistingAcademyTransfer("Burnt Ash Primary School");
        cy.get("h1").should('contain.text',"Burnt Ash Primary School");
        cy.clickBackLink()
        cy.get("h1").should('contain.text',"Manage an academy transfer");
        selectExistingAcademyTransfer("Burnt Ash Primary School");
      
        //  *** Transfer details ***
        // Features of the transfer
        cy.clickDataTest("transfer-features")
        cy.clickBackLink()
        cy.clickDataTest("transfer-features")
        cy.clickDataTest("initiated")
        cy.clickBackLink()
        cy.clickDataTest("initiated")
        selectFirstRadio()
        submit()
        cy.clickDataTest("reason")
        cy.clickBackLink()
        cy.clickDataTest("reason")
        selectFirstRadio()
        cy.fillInText("moreDetail", "Some more detail and reasons")
        submit()
        cy.clickDataTest("type")
        cy.clickBackLink()
        cy.clickDataTest("type")
        selectFirstRadio()
        submit()
        cy.getDataTest("mark-section-complete").uncheck()
        cy.getDataTest("mark-section-complete").check()
        cy.get('.govuk-button').should('contain.text', 'Confirm and continue').click();
        cy.getDataTest("features").should('have.text',"COMPLETED");
        
        // Set transfer dates
        cy.clickDataTest("transfer-dates")
        cy.clickBackLink()
        cy.clickDataTest("transfer-dates")
        cy.clickDataTest("first-discussed")
        cy.clickBackLink()
        cy.clickDataTest("first-discussed")
        cy.fillInDate(Cypress.dayjs().add(-1,'M'))
        submit()
        cy.clickDataTest("target-date")
        cy.clickBackLink()
        cy.clickDataTest("target-date")
        cy.fillInDate(Cypress.dayjs().add(3,'M'))
        submit()
        cy.clickDataTest("ab-date")
        cy.clickBackLink()
        cy.clickDataTest("ab-date")
        cy.fillInDate(Cypress.dayjs().add(2,'M'))
        submit()
        cy.get('.govuk-button').should('contain.text', 'Confirm and continue').click();
        cy.getDataTest("dates").should('have.text',"COMPLETED");

        // Benefits and risks
        cy.clickDataTest("transfer-benefits")
        cy.clickBackLink()
        cy.clickDataTest("transfer-benefits")
        cy.clickDataTest("intended-benefits")
        cy.clickBackLink()
        cy.clickDataTest("intended-benefits")
        cy.get('[type="checkbox"]').check('StrengtheningGovernance')
        cy.get('[type="checkbox"]').check('ImprovingSafeguarding')
        submit()
        cy.clickDataTest("any-risks")
        cy.clickBackLink()
        cy.clickDataTest("any-risks")
        cy.clickDataTest("true")
        submit()
        cy.getDataTest("HighProfile").parent().get("[type='checkbox']").uncheck()
        cy.getDataTest("HighProfile").parent().get("[type='checkbox']").check()
        cy.getDataTest("ComplexLandAndBuildingIssues").parent().get("[type='checkbox']").uncheck()
        cy.getDataTest("ComplexLandAndBuildingIssues").parent().get("[type='checkbox']").check()
        cy.getDataTest("FinanceAndDebtConcerns").parent().get("[type='checkbox']").uncheck()
        cy.getDataTest("FinanceAndDebtConcerns").parent().get("[type='checkbox']").check()
        cy.getDataTest("OtherRisks").parent().get("[type='checkbox']").uncheck()
        cy.getDataTest("OtherRisks").parent().get("[type='checkbox']").check()
        submit()
        cy.fillInText("Answer","High Profile Answer")
        submit()
        cy.fillInText("Answer","Land Answer")
        submit()
        cy.fillInText("Answer","Finance Answer")
        submit()
        cy.fillInText("Answer","Other risks Answer")
        submit()
        cy.getDataTest("mark-section-complete").uncheck()
        cy.getDataTest("mark-section-complete").check()
        cy.get('.govuk-button').should('contain.text', 'Confirm and continue').click()
        cy.getDataTest("benefits").should('have.text',"COMPLETED");    
       
        //Rationale
        cy.clickDataTest("transfer-rationale")
        cy.clickBackLink()
        cy.clickDataTest("transfer-rationale")
        cy.clickDataTest("project-rationale")
        cy.clickBackLink()
        cy.clickDataTest("project-rationale")
        cy.fillInText("ViewModel.ProjectRationale", "this is the project rationale")
        submit();
        cy.clickDataTest("trust-rationale")
        cy.clickBackLink()
        cy.clickDataTest("trust-rationale")
        cy.fillInText("ViewModel.TrustOrSponsorRationale", "this is the project rationale")
        submit();
        cy.getDataTest("mark-section-complete").uncheck()
        cy.getDataTest("mark-section-complete").check()
        cy.get('.govuk-button').should('contain.text', 'Confirm and continue').click();
        cy.getDataTest("rationale").should('have.text',"COMPLETED");

        // Trust information and project dates
        cy.clickDataTest("academy-trust-information")
        cy.clickBackLink()
        cy.clickDataTest("academy-trust-information")
        cy.clickDataTest("recommendation");
        cy.clickBackLink()
        cy.clickDataTest("recommendation");
        selectFirstRadio();
        cy.fillInText("author", "Author name")
        submit();
        cy.get('.govuk-button').should('contain.text', 'Confirm and continue').click();
        cy.getDataTest("academyandtrustinformation").should('have.text',"COMPLETED");

        // *** School Data ***
        cy.clickDataTest("sd-academy-1")
        cy.clickBackLink()
        cy.clickDataTest("sd-academy-1")
        // General infomation
        
        cy.clickDataTest("general-information")
        clickBackLink();
        
        // Pupil numbers
        cy.get(`[data-test='pupil-numbers']`).click()
        cy.clickBackLink()
        cy.get(`[data-test='pupil-numbers']`).click()
        cy.get(`[data-test='additional-information']`).click()
        cy.clickBackLink()
        cy.get(`[data-test='pupil-numbers']`).click()
        cy.get(`[data-test='additional-information']`).click()
        cy.fillInText("AdditionalInformation", "Additional information for pupil numbers");
        submit();
        clickBackLink();
        
        // *** School characteristics ***        
        // Latest Ofsted report
        cy.get(`[data-test='ofsted']`).click()
        clickBackLink();
        cy.get(`[data-test='ofsted']`).click()
        cy.get(`[data-test='additional-information']`).click()
        clickBackLink();
        cy.get(`[data-test='ofsted']`).click()
        cy.get(`[data-test='additional-information']`).click()
        cy.fillInText("AdditionalInformation", "Additional information for ofsted");
        submit();
        clickBackLink()

        // *** School performance ***
        // Key stage 2 performance tables
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
        
        // Key stage 4 performance tables
        cy.get(`[data-test='ks4-performance']`).click()
        clickBackLink()
        cy.get(`[data-test='ks4-performance']`).click()
        cy.get(`[data-test='additional-information']`).click()
        clickBackLink()
        cy.get(`[data-test='ks4-performance']`).click()
        cy.get(`[data-test='additional-information']`).click()
        cy.fillInText("AdditionalInformation", "Additional information for ks4 performance");
        submit();
        clickBackLink()
        
        // Key stage 5 performance tables
        cy.get(`[data-test='ks5-performance']`).click()
        clickBackLink()
        cy.get(`[data-test='ks5-performance']`).click()
        cy.get(`[data-test='additional-information']`).click()
        clickBackLink()
        cy.get(`[data-test='ks5-performance']`).click()
        cy.get(`[data-test='additional-information']`).click()
        cy.fillInText("AdditionalInformation", "Additional information for ks5 performance");
        submit();
        clickBackLink()
        
        // Go to preview
        clickBackLink()
        cy.clickDataTest("preview-htb")
        clickBackLink()
        // Generate project template
        cy.clickDataTest("generate-htb")
        clickBackLink()
        cy.clickDataTest("generate-htb")
        cy.get("[data-test='download-htb']").should($a => {
            expect($a.attr('href'), 'href').to.include("/advisory-board")
            expect($a.attr('target'), 'target').to.equal('_blank')
        })

        // Download Project template
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
