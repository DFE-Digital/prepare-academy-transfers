using Data.TRAMS.Models;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class TramsProjects
    {
        public static TramsProject GetSingleProject()
        {
            return new TramsProject
            {
                ProjectUrn = "001"
            };
        }
    }
}