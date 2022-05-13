// ***********************************************************
// This example support/index.js is processed and
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
import sqlServer from 'cypress-sql-server';

// Alternatively you can use CommonJS syntax:
// require('./commands')
const dayjs = require('dayjs')
Cypress.dayjs = dayjs
sqlServer.loadDBCommands();

beforeEach(() => {
    cy.intercept(
        { url: 'http://localhost:5001/**', middleware: true },
        //Add authorization to all Cypress requests
        (req) => req.headers['Authorization'] = 'Bearer blah'
    )
})