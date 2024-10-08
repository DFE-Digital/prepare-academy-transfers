import { check } from 'k6'

export function checkHeader(page) {

  check(page, {
    'On home page': (p) => p.locator('[data-cy="select-heading"]').textContent().includes('Transfer projects')
  })
}
