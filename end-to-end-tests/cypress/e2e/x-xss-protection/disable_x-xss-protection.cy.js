/// <reference types ='Cypress'/>

Cypress._.each(['ipad-mini'], (viewport) => {
    describe(`113746 X-xss-Protection Header on transfers ${viewport}`, () => {
        beforeEach(() => {
        cy.login()
        cy.viewport(viewport)    
        });
        
        it('TC01: should be disabled', () => {
            cy.url().then(urlString => {
              let modifiedUrl = urlString
              cy.request('GET', modifiedUrl).then((response) => {
                expect(response.headers).to.have.property('x-xss-protection', '0')
              });
            });  
        });
    });
});
