import { defineConfig } from 'cypress'
const { generateZapReport } = require('./cypress/plugins/generateZapReport')
const { verifyDownloadTasks } = require('cy-verify-downloads')

export default defineConfig({

  reporter: 'cypress-multi-reporters',
  reporterOptions: {
    reporterEnabled: 'mochawesome',
    mochawesomeReporterOptions: {
      reportDir: 'cypress/reports/mocha',
      quite: true,
      overwrite: false,
      html: false,
      json: true,
    }
  },

  retries: 0,

  video: false,

  userAgent: 'PrepareTransfers/1.0 Cypress',

  e2e: {
    // eslint-disable-next-line no-unused-vars
    setupNodeEvents(on, config) {
      // implement node event listeners here

      on('after:run', async () => {
        if (process.env.ZAP) {
          await generateZapReport()
        }
      }),

        on('task', verifyDownloadTasks)
    },
  },
})
