class DownloadPage {

  public slug = 'advisory-board/download'

  public downloadProjectTemplate(): this {

    cy.get('h1').should('contain.text', 'Download project template')

    cy.get('[data-test="download-htb"]').click()

    cy.verifyDownload('.docx', { contains: true, timeout: 10000 })

    return this
  }
}

const downloadPage = new DownloadPage()

export default downloadPage
