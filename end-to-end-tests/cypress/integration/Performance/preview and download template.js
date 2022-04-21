const { clear } = require("console");
const fs = require("fs");

let selectFirstRadio = () => {
    cy.get("[type='radio']").then(options => {
        let option = options[0];
        option.click();
    });
}

let selectAllCheckboxesUptoIndex = (index) => {
    cy.get("[type='checkbox']").then(options => {
        for(let i = 0; i < options.length; i++){
            options[i].click()
            if(i >= index){
                break;
            }
        }
    });
}

describe("Performance test for preview and download template with multiple academies", function () {
    let startTimeStamp;
    let endTimeStamp;
    let timeTaken;

	beforeEach(function () {
        startTimeStamp = new Date().getTime();
		cy.login();
	});

    afterEach(() => {
		cy.storeSessionData();
        endTimeStamp = new Date().getTime();
        timeTaken = endTimeStamp - startTimeStamp
        expect(timeTaken,"Time Taken").to.lessThan(15000)
	});
    
    it("Multiple Academies performance test", function () {
        
        

        cy.get("h1").should('contain.text',"Manage an academy transfer");
            cy.get('.govuk-button--start').should('contain.text', 'Start a new transfer project').click()

        cy.get("h1").should('contain.text',"What is the outgoing trust name?");
            cy.get("#SearchQuery").clear().type("bishop fraser")
            cy.get('.govuk-button').should('contain.text', 'Search').click();
        
        cy.get("h1").should('contain.text',"Select the outgoing trust");
            selectFirstRadio()
            cy.get('.govuk-button').should('contain.text', 'Continue').click();
        
        cy.get("h1").should('contain.text',"Outgoing trust details");
            cy.get('.govuk-button').should('contain.text', 'Continue').click();
        
        cy.get("h1").should('contain.text',"Select the transferring academies");
            selectAllCheckboxesUptoIndex(3)
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"What is the incoming trust name?");
            cy.get("[name='query']").clear().type("burnt")
            cy.get('.govuk-button').should('contain.text', 'Search').click();

        cy.get("h1").should('contain.text',"Select an incoming trust");
            selectFirstRadio()
            cy.get('.govuk-button').should('contain.text', 'Continue').click();

        cy.get("h1").should('contain.text',"Check trust and academy details");
            cy.get('button[data-test="create-project"]').click()

        // Go to preview
        cy.get("h2").eq(0).should('contain.text',"Create a project template");
        cy.clickDataTest("preview-htb")
        cy.get("h1").should('contain.text',"Preview project template");       
        
        // Generate project template
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
        endTimeStamp = new Date().getTime();
        cy.log("endTimeStamp = "+endTimeStamp)
        timeTaken = endTimeStamp - startTimeStamp
        cy.log("Time take = " + timeTaken);
    });

    after(function () {
        cy.clearLocalStorage();
    });
});
