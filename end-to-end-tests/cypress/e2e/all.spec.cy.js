/// <reference types ='Cypress'/>

// Run all tests NOTE: this is a work around as cypress 10.0 GUI does not allow to run all tests

// Benefits-and-risks
import './benefits-and-risks/equalities-impact-assessment.cy'

// End2EndTests
import './End2EndTests/project-runthrough-ks2.cy'
import './End2EndTests/project-runthrough.cy'

// Error-messages
import './error-messages/advisory-board-date.cy'
import './error-messages/target-date.cy'

// Landing-page
import './landing-page/landing-page.cy'

// Multiple-academies
import './multiple-academies/select-multiple-academies.cy'

// Performance
import './Performance/preview and download template.cy'

// SQLTests
import './SQLTests/DatabaseTests.cy'

// Trusts-transfers
import './trust-transfers/search-incoming-trust.cy'