/// <reference types ='Cypress'/>

describe('Tests to check equalities impact assessment form',{ tags: '@dev'}, () => {
    afterEach(() => {
        cy.storeSessionData();
    });

    beforeEach(function () {
        cy.visit(`${Cypress.env('url')}project/10002121/benefits`);
    });

    it('Should save yes', () => {
        cy.get('a[data-test=equalities-impact-assessment]').click();
        cy.get('[data-test=true]').click();
        cy.get('[data-test=submit-btn]').click();

        // check Yes has been saved
        cy.get('p[data-test=equalities-impact-assessment]').should($el => expect($el.text()).to.equal('Yes'));

        // back to task list
        cy.get('[href="/project/10002121"]').click();

        // check template preview        
        cy.get('[data-test=preview-htb]').click();
        cy.get('p[data-test=equalities-impact-assessment]').should($el => expect($el.text()).to.equal('Yes'));

        // check change link
        cy.get('a[data-test=equalities-impact-assessment]').click();
        cy.get('[data-test=header]').should($el => expect($el.text().trim()).to.contains(`Has an Equalities Impact Assessment been considered?`));
    });

    it('Should save no', () => {
        cy.get('a[data-test=equalities-impact-assessment]').click();
        cy.get('[data-test=false]').click();
        cy.get('[data-test=submit-btn]').click();

        // check No has been saved
        cy.get('p[data-test=equalities-impact-assessment]').should($el => expect($el.text()).to.equal('No'));

        // back to task list
        cy.get('[href="/project/10002121"]').click();

        // check template preview        
        cy.get('[data-test=preview-htb]').click();
        cy.get('p[data-test=equalities-impact-assessment]').should($el => expect($el.text()).to.equal('No'));
    });

    after(function () {
        cy.clearLocalStorage();
    });
});