import homePage from '../pages/home'

describe('Filter projects', () => {

  const projectTitle = 'Burnt Ash Primary School'

  beforeEach(() => {
    homePage
      .open()
  })

  it('Filters the list of projects', () => {

    const baseCount = homePage.getProjectsCount()
    homePage
      .projectsCountShouldBeVisible()
      .filterProjects(projectTitle)
    const filterCount = homePage.getProjectsCount()
    expect(filterCount < baseCount)
  })

  it('Clears filters', () => {
    const baseCount = homePage.getProjectsCount()
    homePage
      .projectsCountShouldBeVisible()
      .filterProjects(projectTitle)
      .clearFilters()
    const afterClearCount = homePage.getProjectsCount()
    expect(afterClearCount == baseCount)
  })
})