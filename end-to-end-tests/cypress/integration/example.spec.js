describe("Creating an academy transfer", () => {
  let getDataTest = (dataTest) => cy.get(`[data-test='${dataTest}']`)

  let selectFirstRadio = () =>
    cy.get("[type='radio']").then(options => {
      let option = options[0];
      option.click();
    });

  let submit = () =>
    cy.get("[type='submit']")
      .click();

  it("Loads the page", () => {
    cy.visit("/")
    cy.contains("DfE")

    cy.get("[name='username']")
      .type("username")
      .should("have.value", "username");

    cy.get("[name='password']")
      .type("password")
      .should("have.value", "password");

    submit();

    cy.get("[data-test='create-transfer']")
      .click()

    cy.get("[name='query']")
      .type("burnt")

    submit();


    selectFirstRadio();
    submit()

    getDataTest("confirm-outgoing-trust")
      .click();

    selectFirstRadio();
    submit()

    cy.get("[name='query']")
      .type("burnt")

    submit();


    selectFirstRadio();
    
    // Lets not create a billion projects for now
  });
});
