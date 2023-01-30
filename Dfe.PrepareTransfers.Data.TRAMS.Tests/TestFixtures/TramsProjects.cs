using Dfe.PrepareTransfers.Data.TRAMS.Models;

namespace Dfe.PrepareTransfers.Data.TRAMS.Tests.TestFixtures
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