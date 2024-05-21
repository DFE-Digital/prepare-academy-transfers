import { defineConfig } from 'cypress'
const { generateZapReport } = require('./cypress/plugins/generateZapReport')
const { verifyDownloadTasks } = require('cy-verify-downloads')

export default defineConfig({
  video: false,
  userAgent: 'PrepareTransfers/1.0 Cypress',
  
  chromeWebSecurity: false, // Disable Chrome web security to allow cross-origin requests and cookies
  
  e2e: {
    // eslint-disable-next-line no-unused-vars
    setupNodeEvents(on, config) {
      // implement node event listeners here

      on('after:run', async () => {
        if(process.env.ZAP) {
          await generateZapReport()
        }
      }),

      on('task', verifyDownloadTasks)
    },
  },
})
