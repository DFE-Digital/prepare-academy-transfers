import { htmlReport } from 'https://raw.githubusercontent.com/benc-uk/k6-reporter/main/dist/bundle.js'
import { textSummary } from 'https://jslib.k6.io/k6-summary/0.0.1/index.js'
import { browser } from 'k6/experimental/browser'
import { checkHeader } from './pages/home.js'

export const options = {
  scenarios: {
    ui: {
      executor: 'shared-iterations',
      options: {
        browser: {
          type: 'chromium',
        },
      },
    },
  },
  thresholds: {
    checks: ['rate==1.0'],
    browser_http_req_failed: ['rate<0.01']
  },
}

const baseUrl = `${__ENV.BASE_URL}/home`

export default async function () {

  const context = browser.newContext()
  const page = context.newPage()
  page.setExtraHTTPHeaders({
    Authorization: `Bearer ${__ENV.AUTHORIZATION_HEADER}`,
    AuthorizationRole: 'transfers.create'
  })

  try {

    // Go to homepage
    await page.goto(baseUrl)

    await page.locator('[data-cy="select-projectlist-filter-expand"]').click()

    await page.locator('[id="Title"]').type('Rutland Learning Trust')

    await page.locator('[data-cy="select-projectlist-filter-apply"]').click()

    await page.locator('.govuk-notification-banner').isVisible()

    checkHeader(page)

  } finally {
    page.close()
  }
}

export function handleSummary(data) {
  return {
    'summary.html': htmlReport(data),
    stdout: textSummary(data, { indent: ' ', enableColors: true }),
  }
}
