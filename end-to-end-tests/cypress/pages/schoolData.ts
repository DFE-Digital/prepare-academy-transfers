class SchoolDataPage {

  public slug = 'school-data'

  public checkGeneralInformation(): this {

    cy.get('[data-test="general-information"]').click()

    cy.get('h1').should('contain.text', 'General information')

    const giasHeadings = ['School phase', 'Age range', 'Capacity', 'Number on roll (percentage the school is full)', 'Percentage of free school meals (%FSM)']
    const kimFssHeadings = ['Published admission number (PAN)', 'Private finance initiative (PFI) scheme', 'Viability issues', 'Financial deficit', 'School type',
                            'Percentage of good or outstanding academies in the diocesan trust', 'Distance from the academy to the trust headquarters', 'MP (Party)']

    cy.get('dt').then(($keys) => {
      const keys = Cypress._.map($keys, 'innerText')
      expect(keys, 'headings').to.deep.equal(giasHeadings.concat(kimFssHeadings))
    })

    return this
  }

  public checkPupilNumbers(): this {

    cy.get('[data-test="pupil-numbers"]').click()

    cy.get('h1').should('contain.text', 'Pupil numbers')

    const tramsHeadings = ['Girls on roll', 'Boys on roll', 'Pupils with a statement of special educational needs (SEN)', 'Pupils with English as an additional language (EAL)',
                          'Pupils eligible for free school meals during the past 6 years', 'Additional information']

    cy.get('dt').then(($keys) => {
      const keys = Cypress._.map($keys, 'innerText')
      expect(keys, 'headings').to.deep.equal(tramsHeadings)
    })
    
    return this
  }

  public checkOfstedReport(): this {

    cy.get('[data-test="ofsted"]').click()

    cy.get('h1').should('contain.text', 'Latest Ofsted report')

    cy.get('h2').should('contain.text', 'Last full inspection')

    const ofsteadHeadings = ['Latest full inspection date', 'Quality of education', 'Behaviour and attitudes', 'Personal development',
                            'Leadership and management', 'Sixth form provision', 'Ofsted report', 'Additional information']

    cy.get('dt').then(($keys) => {
      const keys = Cypress._.map($keys, 'innerText')
      expect(keys, 'headings').to.deep.equal(ofsteadHeadings)
    })

    return this
  }

  public checkKS4Tables(): this {

    cy.get('[data-test="ks4-performance"]').click()

    cy.get('h1').should('contain.text', 'Key stage 4 performance tables')

    const ks4SectionHeadings = ['Attainment 8', 'Progress 8']

    cy.get('h2.govuk-heading-l').then(($sections) => {
      const keys = Cypress._.map($sections, 'innerText')
      expect(keys, 'headings').to.deep.equal(ks4SectionHeadings)
    })

    return this
  }

  public checkKS5Tables(): this {

    cy.get('[data-test="ks5-performance"]').click()

    cy.get('h1').should('contain.text', 'Key stage 5 performance tables')

    const ks5SectionHeadings = ['2021 to 2022 scores for academic and applied general qualifications',
                                '2020 to 2021 scores for academic and applied general qualifications',
                                '2018 to 2019 scores for academic and applied general qualifications']

    cy.get('h2.govuk-heading-m').then(($sections) => {
      const keys = Cypress._.map($sections, 'innerText')
      expect(keys, 'headings').to.deep.include.members(ks5SectionHeadings)
    })

    return this
  }

  public confirm(): this {

    cy.get('button').contains('Confirm and continue').click()

    return this
  }
}

const schoolDataPage = new SchoolDataPage()

export default schoolDataPage
