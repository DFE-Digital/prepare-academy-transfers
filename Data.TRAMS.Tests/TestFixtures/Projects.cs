using Data.TRAMS.Models;

namespace Data.TRAMS.Tests.TestFixtures
{
    public static class Projects
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