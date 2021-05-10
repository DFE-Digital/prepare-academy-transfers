using System.Collections.Generic;
using API.Models.Upstream.Response;
using Data.Models;

namespace Frontend.Views.Transfers
{
    public class CheckYourAnswers
    {
        public Trust OutgoingTrust;
        public List<GetAcademiesModel> OutgoingAcademies;
        public Trust IncomingTrust;
    }
}