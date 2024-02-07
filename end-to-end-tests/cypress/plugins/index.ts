/// <reference types="cypress" />
// ***********************************************************
// This example plugins/index.js can be used to load plugins
//
// You can change the location of this file or turn off loading
// the plugins file with the 'pluginsFile' configuration option.
//
// You can read more here:
// https://on.cypress.io/plugins-guide
// ***********************************************************

// This function is called when a project is opened or re-opened (e.g. due to
// the project's config changing)

// const fs = require('fs');
// /**
//  * @type {Cypress.PluginConfig}
//  */
// module.exports = (on, config) => {
//     // `on` is used to hook into various events Cypress emits
//     // `config` is the resolved Cypress config
//     on('task', {
//         getDownloadedDoc({path}) {
//             let files = fs.readdirSync(path);
//             return files.find((fileName) => fileName.includes(".docx"))
//         }
//     });
// }

// ***********************************************************

//***Cypress Grep module for filtering tests Any new tags should be added to the examples**
/**
 * @example {{tags: '@dev'} : Development
 * @example {tags: '@stage'} : Staging
 * @example {tags: '@integration'} : Integration
 * @example {tags: ['@dev', '@stage']}
 * @example {tags: '@spike'}
 * @example {tags: '@skip'}
 */
 module.exports = (on, config) => {

  config.baseUrl = config.env.url
  
  require('@cypress/grep/src/plugin')(config)
  return config
}
// ***********************************************************