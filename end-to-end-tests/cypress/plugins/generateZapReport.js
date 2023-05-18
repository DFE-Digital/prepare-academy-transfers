const ZapClient = require('zaproxy')
const fs = require('fs')

module.exports = {
    generateZapReport: async () => {
      const zapOptions = {
        apiKey: process.env.zapApiKey || '',
        proxy: process.env.zapUrl || 'http://localhost:8080'
      }
      const zaproxy = new ZapClient(zapOptions)
      try {
        await zaproxy.core.htmlreport()
        .then(
          resp => {
            if(!fs.existsSync('./reports')) {
              fs.mkdirSync('./reports')
            }
            fs.writeFileSync('./reports/ZAP-Report.html', resp)
          },
          err => {
            console.log(err.message)
          }
        )
      } catch (err) {
        console.log(err)
      }
    }
  }