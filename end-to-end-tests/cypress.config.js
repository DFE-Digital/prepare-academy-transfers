const { defineConfig } = require("cypress");
const { generateZapReport } = require("./cypress/plugins/generateZapReport");

module.exports = defineConfig({
  video: false,
  projectId: "jd9h7w",
  env: {
    db: {
      userName: "",
      password: "",
      server: "",
      options: {
        database: "",
        encrypt: true,
        rowCollectionOnRequestCompletion: true,
      },
    },
  },

  modifyObstructiveCode: true,
  chromeWebSecurity: false,

  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here

      // Map process env vars to cypress vars for usage outside of Cypress run
      on('before:run', () => {
        process.env = config.env
      })

      on('after:run', async () => {
        if(process.env.zapReport) {
          await generateZapReport()
        }
      })
    },
  },
});
