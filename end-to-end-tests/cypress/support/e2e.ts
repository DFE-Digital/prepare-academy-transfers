/* eslint-disable no-undef */
/* eslint-disable no-unused-vars */
// ***********************************************************
// This example support/e2e.js is processed and
// loaded automatically before your test files.
//
// This is a great place to put global configuration and
// behavior that modifies Cypress.
//
// You can change the location of this file or turn off
// automatically serving support files with the
// 'supportFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/configuration
// ***********************************************************

// Import commands.js using ES2015 syntax:
import './commands'
import 'cypress-axe'

// ***********************************************************

// Make dayjs available across all test files
const dayjs = require('dayjs')

declare global {
    namespace Cypress {
        interface Chainable {
            excuteAccessibilityTests(): Chainable<Element>;
        }
        interface Cypress {
            dayjs: dayjs.Dayjs
        }
    }
}

Cypress.dayjs = dayjs

// ***********************************************************

// Add downloaded file verification
require('cy-verify-downloads').addCustomCommand()

// ***********************************************************

// Add auth bypass header before tests run
beforeEach(() => {
    cy.intercept(
        { url: Cypress.env('url') + '**', middleware: true },
        //Add authorization to all Cypress requests
        (req) => {
            req.headers['Authorization'] = 'Bearer ' + Cypress.env('authorizationHeader'),
            req.headers['AuthorizationRole'] = 'transfers.create'
        }
    )
})

// ***********************************************************

//Cypress Grep module for filtering tests
import registerCypressGrep from '@cypress/grep/src/support'
registerCypressGrep()

// ***********************************************************

