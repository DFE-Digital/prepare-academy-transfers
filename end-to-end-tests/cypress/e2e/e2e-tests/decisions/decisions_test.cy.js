describe('Transfer Project Tests', { tags: ['@dev', '@stage'] }, () => {

    beforeEach(() => {
        cy.login();
       });

    it('Creating a transfer project', () => {
        // Step 2: Click on "Create a new transfer project"

        cy.get('[data-test="create-transfer"]'.click());

        

        // Step 3: Click submit
        cy.get('[type="submit"]').click();

   
        // Step 4: Enter "10058252" into the input field
        cy.get('#SearchQuery').type('10058252');

        // Step 5: Select the option with id "10058252" and then click submit
        cy.get('#10058252').click();
        cy.get('[type="submit"]').click();

        // Step 6: Assertion for trust name and Companies House number
        cy.get('.govuk-summary-list__value').should('contain', 'Greater Manchester Academies Trust');
        cy.get('.govuk-summary-list__value').should('contain', '06754335');

        // Step 7: Click continue
        cy.get('[data-test="confirm-outgoing-trust"]').click();

        // Step 8: Select the option with id "10030221"
        cy.get('#10030221').click();

        // Step 9: Click submit
        cy.get('[type="submit"]').click();

        // Step 10: Select the option with id "false", then click continue
        cy.get('#false').click();
        cy.get('[type="submit"]').click();

        // Step 11: Select the option with id "false", then click continue
        cy.get('#false').click();
        cy.get('[type="submit"]').click();

        // Step 12: Click submit
        cy.get('[type="submit"]').click();

        // Step 13: Click on "Record a decision" button
        cy.contains('Record a decision').click();

        // Step 14: Click on "Record a decision" button
        cy.get('#record-decision-link').click();

        // Step 15: Click on "Deferred" radio button, then click submit
        cy.get('#deferred-radio').click();
        cy.get('#submit-btn').click();

        // Step 16: Click on "Grade 6" radio button, then click submit
        cy.get('#grade6-radio').click();
        cy.get('#submit-btn').click();

        // Step 17: Enter "Fahad Darwish" in the text field, then click continue
        cy.get('#decision-maker-name').type('Fahad Darwish');
        cy.get('#submit-btn').click();

        // Step 18: Click on "Performance concerns" checkbox, enter "Cypress Test", then click submit
        cy.get('#performanceconcerns-checkbox').check();
        cy.get('#performanceconcerns-txtarea').type('Cypress Test');
        cy.get('#submit-btn').click();

        // Step 19: Enter date and click submit
        cy.get('#decision-date-day').type('12');
        cy.get('#decision-date-month').type('12');
        cy.get('#decision-date-year').type('2023');
        cy.get('#submit-btn').click();

        // Step 20: Click submit
        cy.get('#submit-btn').click();

        // Step 21: Click submit
        cy.get('#submit-btn').click();

        // Step 22: Click on "Record a decision" and verify the information matches
        cy.contains('Record a decision').click();
        cy.get('#decision').should('contain', 'Deferred');
        cy.get('#decision-made-by').should('contain', 'Deputy Director');
        cy.get('#deferred-reasons').should('contain', 'Performance concerns:');
        cy.get('#deferred-reasons').should('contain', 'Cypress Test');
        cy.get('#decision-date').should('contain', '12 December 2023');
    });

});