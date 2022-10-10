const { defineConfig } = require("cypress");

module.exports = defineConfig({
  projectId: "jd9h7w",
  video: false,

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

  tenantId: "fad277c9-c60a-4da1-b5f3-b3b8b34a82f9",
  clientId: "8b708989-014c-451c-a564-b2bed382f61f",
  modifyObstructiveCode: true,
  chromeWebSecurity: false,

  e2e: {
    setupNodeEvents(on, config) {
      // implement node event listeners here
    },
  },
});
