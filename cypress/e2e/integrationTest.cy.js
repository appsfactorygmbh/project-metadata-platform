Cypress.on('uncaught:exception', (err, runnable) => {
  // returning false here prevents Cypress from failing the test if the login call to the backend fails
  return false;
});

describe('Login Screen', () => {
  it('should load correctly', () => {
    cy.visit('http://127.0.0.1:80/login');
    cy.wait(3000);
    cy.get('h1').should('contain', 'Login');
    cy.get('input[id="standard_login_email"]').should('exist');
    cy.get('input[id="standard_login_password"]').should('exist');
  });
});
