const { defineConfig } = require("cypress");

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
    },
  },
});
