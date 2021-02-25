using System.Collections.Generic;
using API.Models.Upstream.Response;

namespace Frontend.Views.Transfers
{
    public class CheckYourAnswers
    {
        public GetTrustsModel OutgoingTrust;
        public List<GetAcademiesModel> OutgoingAcademies;
        public GetTrustsModel IncomingTrust;
    }
}