// ***********************************************
// This example commands.js shows you how to
// create various custom commands and overwrite
// existing commands.
//
// For more comprehensive examples of custom
// commands please read more here:
// https://on.cypress.io/custom-commands
// ***********************************************
// import "cypress-localstorage-commands";

// Cypress.Commands.add('clickBackLink', () => cy.get('.govuk-back-link').click())
// Cypress.Commands.add('fillInText', (name, text) => cy.get(`[name="${name}"]`).clear().type(text))
// Cypress.Commands.add('fillInTextAtIndex', (index, text) => {
//     cy.get('input[type="text"]:visible').then(options => {
//         let option = options[index];
//         cy.wrap(option).clear().type(text);
//     });
// });

// Cypress.Commands.add('getDataTest', (dataTest) => cy.get(`[data-test='${dataTest}']`))
// Cypress.Commands.add('clickDataTest', (dataTest) => cy.getDataTest(dataTest).click())

// // Record a decision 'Continue' button
// Cypress.Commands.add('continueBtn', () => {
//     cy.get('[id="submit-btn"]')
// })

// Cypress.Commands.add('fillInDate', (dayJs) => {
//     cy.getDataTest("day").clear().type(dayJs.date())
//     cy.getDataTest("month").clear().type(dayJs.month() + 1)
//     cy.getDataTest("year").clear().type(dayJs.year())
// })

// Cypress.Commands.add('fillInDateMonthYear', (dayJs) => {
//     cy.getDataTest("month").clear().type(dayJs.month() + 1)
//     cy.getDataTest("year").clear().type(dayJs.year())
// })


// Cypress.Commands.add('selectCheckbox', (index) => {
//     cy.get("[type='checkbox']").then(options => {
//         let option = options[index]
//         option.click()
//     });
// })

// Cypress.Commands.add('selectRadio', (index) => {
//     cy.get("[type='radio']").then(options => {
//         let option = options[index]
//         option.click()
//     });
// })

// Cypress.Commands.add('storeSessionData', () => {
//     Cypress.Cookies.preserveOnce('.ManageAnAcademyConversion.Login')
//     let str = [];
//     cy.getCookies().then((cookie) => {
//         cy.log(cookie);
//         for (let l = 0; l < cookie.length; l++) {
//             if (cookie.length > 0 && l == 0) {
//                 str[l] = cookie[l].name;
//                 Cypress.Cookies.preserveOnce(str[l]);
//             } else if (cookie.length > 1 && l > 1) {
//                 str[l] = cookie[l].name;
//                 Cypress.Cookies.preserveOnce(str[l]);
//             }
//         }
//     });
// })

// Cypress.Commands.add("login", () => {
//     cy.visit(Cypress.env('url') + 'project-type')
//     cy.get('[id="transfer-radio"]').click()
//     cy.get('[id="submit-btn"]').click()
// })


// ***********************************************
/*  by Asif Ali

    This file is going to be a collection of buttons and links for each page of the transfer journey.
    The idea is using the benefits of the pageObject model used in various JAVA/C# testing frameworks.
    functions will be named according to how they look on the page, and we can identify very easily the error
    should one ocur. Also if code changes, we only have to change in one place instead of every instance.
*/

/*
    Create Journey
*/
// Shared Identifiers
// let backLink = '.govuk-back-link'
// PAGE: Manage an academy transfer 
// Cypress.Commands.add('HomePage_Button_StartNewProject', () => cy.get('.govuk-button--start'))
// Cypress.Commands.add('HomePage_Link_Back', () => cy.get(`${backLink}`))
// PAGE: What is the outgoing trust name?
// Cypress.Commands.add('OutGoingSearch_Button_Search', () => cy.get('.govuk-button'))
// Cypress.Commands.add('OutGoingSearch_Link_Back', () => cy.get(`${backLink}`))
// PAGE: Select the outgoing trust
// PAGE: Outgoing trust details
// PAGE: Select the transferring academies 
// PAGE: What is the incoming trust name?
// PAGE: Select the incoming trust
// PAGE: Check trust and academy details

/*
    Edit Journey
*/
// PAGE: Transfer Project Main
// PAGE: Feature of Transfer
// PAGE: Set transfer dates
// PAGE: Benefits and risks
// PAGE: Rationale
// PAGE: Trust information and project dates

// Universal: Navigate to Transfer and pick the first project

// Selects Transfers by default: universal
// Cypress.Commands.add('selectsTransferLandingPage', () => {
//     let url = Cypress.env('url');
//     cy.visit(url);
//     cy.get('[data-cy="select-projecttype-input-transfer"]').click();
//     cy.get('[data-cy="select-common-submitbutton"]').click();
// })

// Selects the first project on the list: universal
// Cypress.Commands.add('selectsFirstProjectOnList', () => {

//     let selectExistingAcademyTransfer = (projectName) =>
//     cy.get('[class="govuk-link"]').then(projects => {
//         for (let i = 0; i < projects.length; i++) {
//             if (projects[i].text.includes("Burnt Ash Primary School")) {
//                 projects[i].click();
//                 break;
//             }
//         }
//     });

//     let projectUrl = "";

//     let url = Cypress.env('url');
//     cy.visit(url);
//     cy.get('[data-cy="select-projecttype-input-transfer"]').click();
//     cy.get('[data-cy="select-common-submitbutton"]').click();
//     selectExistingAcademyTransfer("Burnt Ash Primary School");
// })

//--Legal Requirements (Task List)

// Save & Continue btn (Universal)
// Cypress.Commands.add('saveAndContinueButton', () => {
//     cy.get('[data-test="submit-btn"]')
// })
// Incoming Trust Agreement: Change Link
// Cypress.Commands.add('incomingTrustAgreementLink', () => {
//     cy.get('a[href*="incoming-trust-agreement"]').click()
// })

// Incoming Trust Agreement: status
// Cypress.Commands.add('incomingTrustAgreementStatus', () => {
//     cy.get('[data-test="incoming-trust-agreement"]')
// })

// Diocesan Consent: Change Link
// Cypress.Commands.add('diocesanConsentLink', () => {
//     cy.get('a[href*="diocesan-consent"]').click()
// })

// Diocesan Consent: Status
// Cypress.Commands.add('diocesanConsentStatus', () => {
//     cy.get('[data-test="diocesan-consent"]')
// })

// Outgoing Trust Consent: Change Link
// Cypress.Commands.add('outgoingTrustConsentLink', () => {
//     cy.get('a[href*="outgoing-trust-consent"]').click()
// })

// Outgoing Trust Consent: Status
// Cypress.Commands.add('outgoingTrustConsentStatus', () => {
//     cy.get('[data-test="outgoing-trust-consent"]')
// })

// Trust Listing Summary Page (Universal)
// Cypress.Commands.add('selectTrustListing', (listing) => {
//     cy.get('[data-id^="project-link-"]').first().click()
//     cy.get('*[href*="features"]').should('be.visible')
//     // cy.saveLocalStorage()
// });

//Navigate To Filter Projects section
// Cypress.Commands.add('navigateToFilterProjects',() => {  
//     cy.get('[data-cy="select-projectlist-filter-expand"]').click();
//     cy.get('[data-id="filter-container"]').should('be.visible');
// });

// Universal: selects first project from list
// Cypress.Commands.add('selectFirstProject', () => {
//     cy.get(':nth-child(1) > :nth-child(1) > .govuk-caption-l > strong > .govuk-link').click();
//     cy.url().should('include', '/project');
// });

// Unassign a user
// Cypress.Commands.add('unassignUser', () => {
//     cy.get('[data-id="assigned-user"]')
//       .invoke('text')
//       .then((text) => {
//         if (text.includes('Empty')) {
//           return
//         }
//         else {
//           // assign link
//           cy.get('a[href*="project-assignment"]').click();
//           // unassign link
//           cy.get('[id="unassign-link"]').click();
//           // continue button
//           cy.get('[class="govuk-button"]').click();
//         }
//     });
// });

// Universal: selects conversion project from list
// Cypress.Commands.add('selectsTransfers', () => {
//     let url = Cypress.env('url');
//     cy.visit(url);
//     cy.get('[data-cy="select-projecttype-input-transfer"]').click();
//     cy.get('[data-cy="select-common-submitbutton"]').click();
// });

// Assign User
// Cypress.Commands.add('assignUser', () => {
//     cy.get('[data-id="assigned-user"]')
//       .invoke('text')
//       .then((text) => {
//         if (text.includes('Empty')) {
//           cy.get('a[href*="project-assignment"]').click();
//           cy.get('[id="delivery-officer"]').click().type('Richika Dogra').type('{enter}');
//           cy.get('[class="govuk-button"]').click();
//         }
//     });
// });

// Check accessibility
Cypress.Commands.add('excuteAccessibilityTests', (wcagStandards, continueOnFail, impactLevel) => {
    cy.injectAxe();
    cy.checkA11y(null, {
        runOnly: {
            type: 'tag',
            values: wcagStandards
        },
        includedImpacts: impactLevel
    }, null, continueOnFail);
})
