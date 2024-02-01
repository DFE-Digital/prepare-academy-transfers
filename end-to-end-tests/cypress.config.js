const { defineConfig } = require("cypress");
const { generateZapReport } = require("./cypress/plugins/generateZapReport");

module.exports = defineConfig({
  video: false,
  // env: {
  //   db: {
  //     userName: "",
  //     password: "",
  //     server: "",
  //     options: {
  //       database: "",
  //       encrypt: true,
  //       rowCollectionOnRequestCompletion: true,
  //     },
  //   },
  // },
  userAgent: 'PrepareTransfers/1.0 Cypress',

  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here

      on('after:run', async () => {
        if(process.env.ZAP) {
          await generateZapReport()
        }
      })
    },
  },
});
