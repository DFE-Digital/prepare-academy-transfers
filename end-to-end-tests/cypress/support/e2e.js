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

const dayjs = require('dayjs')
Cypress.dayjs = dayjs

beforeEach(() => {
    cy.intercept(
        { url: Cypress.env('url') + '**', middleware: true },
        //Add authorization to all Cypress requests
        (req) => req.headers['Authorization'] = 'Bearer ' + Cypress.env('authorizationHeader'),
        (req) => req.headers['AuthorizationRole'] = 'transfers.create'
    )
})

// ***********************************************************

//Cypress Grep module for filtering tests
import registerCypressGrep from '@cypress/grep/src/support'
registerCypressGrep()

// ***********************************************************
import 'cypress-axe'
