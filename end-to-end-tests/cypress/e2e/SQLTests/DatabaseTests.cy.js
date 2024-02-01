/*
    This was an attempt to try and create some tests or methods that can coneect 
*/
describe.skip('Example to Demonstrate SQL Database Testing in Cypress', { tags: '@spike'}, () => {

    it.skip('TEST CONNECTION with Pre-Prod', function () {
        cy.sqlServer("SELECT TOP (2) * FROM [cdm].[contact]").then((result) => {
            cy.log(result[0][0] + " | " + result[0][1]);
            cy.log(result[1][0] + " | " + result[1][1]);
        })
    })
})