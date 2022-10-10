/// <reference types ='Cypress'/>
// Run all tests NOTE: this is a work around as cypress 10.0 GUI does not allow to run all tests

// Benefits-and-risks
import '../e2e/benefits-and-risks/equalities-impact-assessment.cy'

// End2EndTests
import '../e2e/End2EndTests/project-runthrough-ks2.cy'
import '../e2e/End2EndTests/project-runthrough.cy'

// Error-messages
import '../e2e/error-messages/advisory-board-date.cy'
import '../e2e/error-messages/target-date.cy'

// Landing-page
import '../e2e/landing-page/landing-page.cy'

// Multiple-academies
import '../e2e/multiple-academies/select-multiple-academies.cy'

// Performance
import '../e2e/Performance/preview and download template.cy'

// SQLTests
import '../e2e/SQLTests/DatabaseTests.cy'

// Trusts-transfers
import '../e2e/trust-transfers/search-incoming-trust.cy'