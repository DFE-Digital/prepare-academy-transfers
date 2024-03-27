using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures
{
    public static class TramsProjects
    {
        public static AcademisationProject GetSingleProject()
        {
            return new AcademisationProject
            {
                ProjectUrn = "001"
            };
        }
    }
}