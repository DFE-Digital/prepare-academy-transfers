using Dfe.PrepareTransfers.Web.Models;

namespace Dfe.PrepareTransfers.Web.Pages.Transfers
{
    public abstract class TransfersPageModel : CommonPageModel
    {
        protected const string OutgoingAcademyIdSessionKey = "OutgoingAcademyIds";
        protected const string IncomingTrustIdSessionKey = "IncomingTrustId";
        protected const string OutgoingTrustIdSessionKey = "OutgoingTrustId";
        protected const string ProjectCreatedSessionKey = "ProjectCreated";
    }
}
